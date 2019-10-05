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
        //Отлов событий подключения/отключения usb устройств
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
            UpdateDevices(false);
        }

        private void UpdateDevices(bool first)
        {
            devices.CheckStart(); //Начало проверки устройств на актуальность
            //List<string> disksName = new List<string>(); //Буквы дисков

            //Выборка букв USB устройств
            //Новый простой способ
            DriveInfo[] D = DriveInfo.GetDrives();
            foreach (DriveInfo DI in D)
            {
                if (DI.DriveType == DriveType.Removable)
                { 
                    //Проверка устройств на Garmin
                    if (File.Exists(Convert.ToString(DI.Name) + Path.GarminXml))
                    {
                        string id = null;
                        string model = null;
                        string nickname = null;

                        //Загрузка данных устройства из XML файла
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(Convert.ToString(DI.Name) + Path.GarminXml);
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
                                if (!first && Properties.Settings.Default.SoundConnect)
                                    new System.Media.SoundPlayer("sounds/connect.wav").Play();
                                devices.Add(id, nickname, Convert.ToString(DI.Name), model); //Добавление устройства
                            }
                    }
                }
            }
            devices.CheckEnd(); //Конец проверки устройств на актуальность (не актуальные удаляются)
            //Старый способ
            //foreach (System.Management.ManagementObject drive in
            //        new System.Management.ManagementObjectSearcher(
            //        "select * from Win32_DiskDrive where InterfaceType='USB'").Get())
            //{
            //    foreach (System.Management.ManagementObject partition in
            //        new System.Management.ManagementObjectSearcher(
            //        "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"]
            //        + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
            //    {
            //        foreach (System.Management.ManagementObject disk in
            //            new System.Management.ManagementObjectSearcher(
            //            "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='"
            //            + partition["DeviceID"]
            //            + "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
            //        {
            //            disksName.Add(disk["Name"].ToString().Trim()); //Запись буквы устройства в список
            //        }
            //    }
            //}
            
        }
        #endregion

        public WindowMain()
        {
            InitializeComponent();

            CheckBoxSoundConnect.IsChecked = Properties.Settings.Default.SoundConnect;
            CheckBoxSoundReady.IsChecked = Properties.Settings.Default.SoundReady;
            CheckBoxSoundDisconnect.IsChecked = Properties.Settings.Default.SoundDisconnect;

            logs = new UserControlLogs();
            logs.Visibility = Visibility.Hidden;
            GridContent.Children.Add(logs);

            devices = new UserControlDevices(ref logs);
            devices.Visibility = Visibility.Visible;
            GridContent.Children.Add(devices);            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Hwnd start
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
            UpdateDevices(true);
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

        private void CheckBoxSoundConnect_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SoundConnect = true;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxSoundConnect_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SoundConnect = false;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxSoundReady_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SoundReady = true;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxSoundReady_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SoundReady = false;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxSoundDisconnect_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SoundDisconnect = true;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxSoundDisconnect_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SoundDisconnect = false;
            Properties.Settings.Default.Save();
        }
    }
}
