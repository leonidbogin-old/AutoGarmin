using AutoGarmin.Class;
using AutoGarmin.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;
using System.Xml.Linq;

namespace AutoGarmin
{
    public partial class MainWindow : Window
    {
        private MainMenu mainMenu;
        private UserControlMap userControlMap;
        private UserControlTrack userControlTrack;
        private UserControlSettings userControlSettings;

        public MainWindow()
        {
            InitializeComponent();
            MainMenuLoad(MainMenu.menu.Map);
        }

        private void MainMenuLoad(MainMenu.menu i)
        {
            if (mainMenu != null) mainMenu.i = i;
            else mainMenu = new MainMenu(i);

            GridContent.Children.Clear();
            switch (mainMenu.i)
            {
                case MainMenu.menu.Map:
                    ButtonMenuMap.Style = (Style)FindResource("ButtonMenuActive");
                    if (userControlMap == null) userControlMap = new UserControlMap();
                    GridContent.Children.Add(userControlMap);
                        break;
                case MainMenu.menu.Track:
                    ButtonMenuTrack.Style = (Style)FindResource("ButtonMenuActive");
                    if (userControlTrack == null) userControlTrack = new UserControlTrack();
                    GridContent.Children.Add(userControlTrack);
                    break;
                case MainMenu.menu.Settings:
                    ButtonMenuSettings.Style = (Style)FindResource("ButtonMenuActive");
                    if (userControlSettings == null) userControlSettings = new UserControlSettings();
                    GridContent.Children.Add(userControlSettings);
                        break;
            }
        }

        private void MainMenuClick(MainMenu.menu i)
        {
            if (i != mainMenu.i)
            {
                Style ButtonMenu = (Style)FindResource("ButtonMenu");
                if (ButtonMenuMap.Style != ButtonMenu) ButtonMenuMap.Style = ButtonMenu;
                if (ButtonMenuTrack.Style != ButtonMenu) ButtonMenuTrack.Style = ButtonMenu;
                if (ButtonMenuSettings.Style != ButtonMenu) ButtonMenuSettings.Style = ButtonMenu;
                MainMenuLoad(i);
            }
        }

        private void ButtonMenuMap_Click(object sender, RoutedEventArgs e)
        {
            MainMenuClick(MainMenu.menu.Map);
        }

        private void ButtonMenuTrack_Click(object sender, RoutedEventArgs e)
        {
            MainMenuClick(MainMenu.menu.Track);
        }

        private void ButtonMenuSettings_Click(object sender, RoutedEventArgs e)
        {
            MainMenuClick(MainMenu.menu.Settings);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == USB.Code.WM_DEVICECHANGE)
            {
                if (wParam.ToInt32() == USB.Code.DBT_DEVICEARRIVAL
                    || wParam.ToInt32() == USB.Code.DBT_DEVICEREMOVECOMPLETE)
                {
                    Action update = () => UpdateDevices();
                    this.Dispatcher.BeginInvoke(update);
                }
            }
            return IntPtr.Zero;
        }


        #region Devices
        public List<UserControlDevice> deviceList = new List<UserControlDevice>(); //device list

        private void UpdateDevices()
        {
            var doubleAnimation = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(1)));
            var rotateTransform = new RotateTransform();
            ImageUpdateDevice.RenderTransform = rotateTransform;
            ImageUpdateDevice.RenderTransformOrigin = new Point(0.5, 0.5);
            doubleAnimation.RepeatBehavior = new RepeatBehavior(1);
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, doubleAnimation);               

            List<UserControlDevice> deviceCheck = deviceList.GetRange(0, deviceList.Count);

            //USB device letter sampling
            DriveInfo[] D = DriveInfo.GetDrives();
            foreach (DriveInfo DI in D)
            {
                if (DI.DriveType == DriveType.Removable)
                {
                    //Checking devices on Garmin device
                    if (File.Exists(Convert.ToString(DI.Name) + Const.Path.GarminXml))
                    {
                        string id = null;
                        string model = null;
                        string nickname = null;
                        string icon = null;

                        //Loading device data from an XML file
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(Convert.ToString(DI.Name) + Const.Path.GarminXml);
                        XmlElement xRoot = xDoc.DocumentElement;

                        bool find = false;
                        foreach (XmlNode xnode in xRoot)
                        {
                            if (xnode.Name == Const.Xml.Id)
                            {
                                id = xnode.InnerText;
                                for (int i = 0; i < deviceCheck.Count; i++)
                                {
                                    if (deviceCheck[i].deviceInfo.id == id) //already have
                                    {
                                        find = true;
                                        deviceCheck.Remove(deviceCheck[i]);
                                        break;
                                    }
                                }
                            }
                            else if (xnode.Name == Const.Xml.Model)
                            {
                                foreach (XmlNode xmodel in xnode)
                                {
                                    if (xmodel.Name == Const.Xml.Description)
                                    {
                                        model = xmodel.InnerText;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!find) //not yet
                        {
                            //additional information
                            if (File.Exists(Convert.ToString(DI.Name) + Const.Path.GarminInformationXml))
                            {
                                XmlDocument xDocInfo = new XmlDocument();
                                xDocInfo.Load(Convert.ToString(DI.Name) + Const.Path.GarminInformationXml);
                                XmlElement xRootInfo = xDocInfo.DocumentElement;
                                foreach (XmlNode xnode in xRootInfo)
                                {
                                    if (xnode.Name == Const.Xml.Nickname)
                                    {
                                        nickname = xnode.InnerText;
                                        break;
                                    }
                                }
                            }

                            //get ico from autorun
                            if (File.Exists(Convert.ToString(DI.Name) + Const.Path.GarminAutorun))
                            {
                                try
                                {
                                    FileStream finput = new FileStream(Convert.ToString(DI.Name) + Const.Path.GarminAutorun, FileMode.Open);
                                    StreamReader fin = new StreamReader(finput);
                                    while (!fin.EndOfStream)
                                    {
                                        string buff = fin.ReadLine();
                                        if (buff.StartsWith(Const.Path.GarminAutorunImage))
                                        {
                                            icon = buff.Substring(Const.Path.GarminAutorunImage.Length);
                                            break;
                                        }
                                    }
                                    fin.Close();
                                }
                                catch
                                {

                                }
                            }

                            DeviceInfo deviceInfo = new DeviceInfo()
                            {
                                id = id,
                                nickname = nickname,
                                diskname = Convert.ToString(DI.Name),
                                model = model,
                                icon = icon,
                                timeConnect = DateTime.Now
                            };
                            UserControlDevice userControlDevice = new UserControlDevice(deviceInfo);
                            deviceList.Add(userControlDevice);
                            StackPanelDevices.Children.Add(userControlDevice);
                        }
                    }
                }
            }
            //need to delete
            for (int i = 0; i < deviceCheck.Count; i++)
            {
                deviceList.Remove(deviceCheck[i]);
                StackPanelDevices.Children.Remove(deviceCheck[i]);
                deviceCheck.Remove(deviceCheck[i]);
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
            UpdateDevices();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            Action update = () => UpdateDevices();
            this.Dispatcher.BeginInvoke(update);
        }
    }
}

//XDocument xdoc = new XDocument();
//XElement GarminInformation = new XElement(Const.Xml.GarminInformation);
//XAttribute iphoneNameAttr = new XAttribute(Const.Xml.Nickname, "");