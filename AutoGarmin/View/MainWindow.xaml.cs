using AutoGarmin.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace AutoGarmin
{
    public partial class MainWindow : Window
    {
        public const string GPXPath = "GPX";

        #region View

        private UserControlDevices userControlDevices;
        private UserControlLogs userControlLogs;

        #endregion

        #region USBDevices

        const int WM_DEVICECHANGE = 0x0219; //что-то связанное с usb
        const int DBT_DEVICEARRIVAL = 0x8000; //устройство подключено
        const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // устройство отключено

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                if (wParam.ToInt32() == DBT_DEVICEARRIVAL 
                    || wParam.ToInt32() == DBT_DEVICEREMOVECOMPLETE)
                {
                    Dispatcher.BeginInvoke(new MethodInvoker(delegate
                    {
                        UpdateDevices();
                    }));
                }
            }
            return IntPtr.Zero;
        }

        private void UpdateDevices()
        {
            userControlDevices.CheckStart();

            List<string> disksName = new List<string>();

            foreach (System.Management.ManagementObject drive in
                    new System.Management.ManagementObjectSearcher(
                    "select * from Win32_DiskDrive where InterfaceType='USB'").Get())
            {
                foreach (System.Management.ManagementObject partition in
                    new System.Management.ManagementObjectSearcher(
                    "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"]
                    + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
                {
                    foreach (System.Management.ManagementObject disk in
                        new System.Management.ManagementObjectSearcher(
                        "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='"
                        + partition["DeviceID"]
                        + "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
                    {
                        disksName.Add(disk["Name"].ToString().Trim());
                    }
                }
            }

            foreach (string diskName in disksName)
            {
                if (File.Exists(diskName + @"\Garmin\GarminDevice.xml"))
                {
                    
                    string id = null;
                    string model = null;

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(diskName + @"\Garmin\GarminDevice.xml");
                    XmlElement xRoot = xDoc.DocumentElement;

                    foreach (XmlNode xnode in xRoot)
                    {
                        if (xnode.Name == "Id") id = xnode.InnerText;
                        if (xnode.Name == "Model")
                        {
                            foreach (XmlNode xmodel in xnode)
                            {
                                if (xmodel.Name == "Description") model = xmodel.InnerText;
                            }
                        }
                    }

                    if (id != null)
                        if (!userControlDevices.Check(id))
                        {
                            userControlDevices.Add(id, "1-1", diskName, model);
                        }
                }
            }

            userControlDevices.CheckEnd();
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            userControlLogs = new UserControlLogs();
            userControlDevices = new UserControlDevices(userControlLogs);

            userControlDevices.Visibility = Visibility.Visible;
            userControlLogs.Visibility = Visibility.Hidden;

            GridContent.Children.Add(userControlDevices);
            GridContent.Children.Add(userControlLogs);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
            UpdateDevices();
        }

        #region MainButton Event

        private void ButtonDevices_Click(object sender, RoutedEventArgs e)
        {
            if (userControlDevices.Visibility == Visibility.Hidden)
            {
                userControlDevices.Visibility = Visibility.Visible;
                userControlLogs.Visibility = Visibility.Hidden;
                ButtonLogs.Style = (Style)FindResource("MainButton");
                ButtonDevices.Style = (Style)FindResource("MainButton_Active");
            }
        }

        private void ButtonLogs_Click(object sender, RoutedEventArgs e)
        {
            if (userControlLogs.Visibility == Visibility.Hidden)
            {
                userControlLogs.Visibility = Visibility.Visible;
                userControlDevices.Visibility = Visibility.Hidden;
                ButtonDevices.Style = (Style)FindResource("MainButton");
                ButtonLogs.Style = (Style)FindResource("MainButton_Active");
            }
        }

        private void ButtonTrackFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(GPXPath)) Directory.CreateDirectory(GPXPath);
            Process Proc = new Process();
            Proc.StartInfo.FileName = "explorer";
            Proc.StartInfo.Arguments = GPXPath;
            Proc.Start();
            Proc.Close();
        }

        #endregion

        private void DataGridContentUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateDevices();
        }
    }
}
