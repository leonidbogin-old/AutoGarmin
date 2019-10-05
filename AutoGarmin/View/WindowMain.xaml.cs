using AutoGarmin.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Xml;

namespace AutoGarmin
{
    public partial class WindowMain : Window
    {
        #region View
        private UserControlDevices devices;
        private UserControlLogs logs;
        #endregion

        #region USBDevices
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == USB.Code.WM_DEVICECHANGE)
            {
                if (wParam.ToInt32() == USB.Code.DBT_DEVICEARRIVAL 
                    || wParam.ToInt32() == USB.Code.DBT_DEVICEREMOVECOMPLETE)
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
            devices.CheckStart(); //Начало проверки устройств на актуальность
            List<string> disksName = new List<string>(); //Буквы дисков
            
            //Выборка USB устройств
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
                        disksName.Add(disk["Name"].ToString().Trim()); //Запись буквы устройства в список
                    }
                }
            }

            //Проверка устройств на Garmin
            foreach (string diskName in disksName)
            {
                if (File.Exists(diskName + Path.GarminXml))
                {
                    string id = null;
                    string model = null;
                    string nickname = null;

                    //Загрузка данных устройства из XML файла
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(diskName + Path.GarminXml);
                    XmlElement xRoot = xDoc.DocumentElement;

                    foreach (XmlNode xnode in xRoot)
                    {
                        if (xnode.Name == "Id") id = xnode.InnerText;
                        else if (xnode.Name == "Model")
                        {
                            foreach (XmlNode xmodel in xnode)
                            {
                                if (xmodel.Name == "Description") model = xmodel.InnerText;
                            }
                        }
                        else if (xnode.Name == "Nickname") nickname = xnode.InnerText;
                    }

                    if (id != null)
                        if (!devices.Check(id))
                        {
                            devices.Add(id, nickname, diskName, model); //Добавление устройства
                        }
                }
            }
            devices.CheckEnd(); //Конец проверки устройств на актуальность (не актуальные удаляются)
        }
        #endregion

        public WindowMain()
        {
            InitializeComponent();

            logs = new UserControlLogs();
            logs.Visibility = Visibility.Hidden;
            GridContent.Children.Add(logs);

            devices = new UserControlDevices(ref logs);
            devices.Visibility = Visibility.Visible;
            GridContent.Children.Add(devices);            
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
            if (devices.Visibility == Visibility.Hidden)
            {
                devices.Visibility = Visibility.Visible;
                logs.Visibility = Visibility.Hidden;
                ButtonLogs.Style = (Style)FindResource("MainButton");
                ButtonDevices.Style = (Style)FindResource("MainButton_Active");
            }
        }

        private void ButtonLogs_Click(object sender, RoutedEventArgs e)
        {
            if (logs.Visibility == Visibility.Hidden)
            {
                logs.Visibility = Visibility.Visible;
                devices.Visibility = Visibility.Hidden;
                ButtonDevices.Style = (Style)FindResource("MainButton");
                ButtonLogs.Style = (Style)FindResource("MainButton_Active");
            }
        }

        private void ButtonTrackFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(Path.GPX)) Directory.CreateDirectory(Path.GPX);
            Process Proc = new Process();
            Proc.StartInfo.FileName = "explorer";
            Proc.StartInfo.Arguments = Path.GPX;
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
