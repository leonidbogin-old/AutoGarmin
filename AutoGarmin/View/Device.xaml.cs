using AutoGarmin.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml;

namespace AutoGarmin
{
    public partial class Device : UserControl
    {
        private View.Logs logs; //link to log
        public DeviceInfo deviceInfo; //device information
        public WindowDeviceRename deviceRenameWindow; //window of renaming the device

        public Device(DeviceInfo deviceInfo, ref View.Logs logs) //start
        { 
            this.logs = logs; //savelink
            this.deviceInfo = deviceInfo;
            InitializeComponent();
            LabelModel.Content = $"{deviceInfo.model} ({deviceInfo.diskname})"; //output model
            if (deviceInfo.nickname != null && deviceInfo.nickname.Length > 0) //output nickname
                LabelNickname.Content = deviceInfo.nickname;
            else LabelNickname.Content = Const.Label.NoNickname;
            LabelId.Content = deviceInfo.id; //output id
            LabelTimeConnect.Content = deviceInfo.timeConnect.ToString(Const.Time.Connect); //output connectшщт time
            ImageDevice.Source = LoadIco(deviceInfo.diskname); //output device image
        }

        private BitmapImage LoadIco(string diskname) //loading device icon
        {
            BitmapImage bm1 = new BitmapImage();

            string path = diskname + Const.Path.GarminIco;
            if (!File.Exists(path))
            {
                //If there is no standard ico, look for any other .ico on the device
                List<string> files = new List<string>();
                try
                {
                    FilesWork.GetAllFiles(diskname + "/", "*.ico", ref files);
                }
                catch (UnauthorizedAccessException) { }
                if (files.Count > 0)
                    path = files[0];
                else path = Const.Path.NoIco; //Otherwise, put the picture of the resources 'no.ico'
            }
            bm1.BeginInit();
            bm1.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bm1.CacheOption = BitmapCacheOption.OnLoad;
            bm1.EndInit();

            return bm1;
        }

        public void StartAuto() //start auto mode
        {
            deviceInfo.ready = false;
            deviceInfo.warning = false;
            deviceInfo.error = false;
            RectangleDeviceStatus.ToolTip = "";
            RectangleDeviceStatus.Fill = Const.Color.DeviceStartAuto();
            logs.Add(deviceInfo, Const.Log.DeviceStartAuto);
            System.Threading.Thread myThread = new System.Threading.Thread(
                new System.Threading.ThreadStart(Auto));
            myThread.Start();
        }

        private void Auto() //auto mode
        {
            if (Properties.Settings.Default.AutoTracksDownload)
            {
                CopyTracks();
            }
            if (Properties.Settings.Default.AutoTracksClean)
            {
                CleanTracks();
            }
            if (Properties.Settings.Default.AutoMapClean)
            {
                CleanMaps();
            }
            if (Properties.Settings.Default.AutoMapLoad)
            {
                LoadMap();
            }
            deviceInfo.ready = true;
            this.Dispatcher.Invoke((System.Threading.ThreadStart)delegate {
                if (deviceInfo.error)
                {
                    RectangleDeviceStatus.Fill = Const.Color.Error();
                    RectangleDeviceStatus.ToolTip = Const.Label.ToolTip.Error;
                }
                else if (deviceInfo.warning)
                {
                    RectangleDeviceStatus.Fill = Const.Color.Warning();
                    RectangleDeviceStatus.ToolTip = Const.Label.ToolTip.Warning;
                }
                else if (deviceInfo.ready)
                {
                    RectangleDeviceStatus.Fill = Const.Color.DeviceReady();
                    RectangleDeviceStatus.ToolTip = Const.Label.ToolTip.Ready;
                }

                if (!deviceInfo.error && deviceInfo.ready)
                {
                    if (Properties.Settings.Default.SoundReady) Sound.Play(Const.Path.Sound.Ready);
                    logs.Add(deviceInfo, Const.Log.DeviceEndAuto);
                }
            });
        }

        public void CopyTracks() //copying tracks
        {
            try
            {
                if (Directory.Exists(deviceInfo.diskname + Const.Path.GPXPath))
                {
                    string pathTarget = Const.Path.GPX + @"\" + DateTime.Now.ToShortDateString()
                        + @"\(" + DateTime.Now.ToString(Const.Time.Folder) + ")";
                    if (deviceInfo.nickname != null && deviceInfo.nickname.Length > 0)
                        pathTarget += $" - ({deviceInfo.nickname})";
                    if (deviceInfo.model != null && deviceInfo.model.Length > 0)
                        pathTarget += $" - ({deviceInfo.model})";
                    pathTarget += $" - ({deviceInfo.id})";
                    DirectoryInfo dirSource = new DirectoryInfo(deviceInfo.diskname + Const.Path.GPXPath);
                    DirectoryInfo dirTrarget = new DirectoryInfo(pathTarget);
                    FilesWork.Folder.Copy(dirSource, dirTrarget);
                    logs.Add(deviceInfo, Const.Log.DeviceTracksDownload);
                }
                else
                {
                    deviceInfo.error = true;
                    if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                    logs.Add(deviceInfo, Const.Log.Error.NoTracksFolder);
                }
            }
            catch
            {
                deviceInfo.error = true;
                if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                logs.Add(deviceInfo, Const.Log.Error.DeviceTracksDownload);
            }
        }


        public void CleanTracks() //removal of tracks
        {
            try
            {
                if (Directory.Exists(deviceInfo.diskname + Const.Path.GPXPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(deviceInfo.diskname + Const.Path.GPXPath);
                    FilesWork.Folder.Clean(dir);
                    logs.Add(deviceInfo, Const.Log.DeviceTracksClean);
                }
                else
                {
                    deviceInfo.warning = true;
                    logs.Add(deviceInfo, Const.Log.Error.NoTracksFolderClean);
                }
            }
            catch
            {
                deviceInfo.warning = true;
                logs.Add(deviceInfo, Const.Log.Error.DeviceTracksClean);
            }
        }

        public void CleanMaps() //removal of maps
        {
            try
            {
                if (Directory.Exists(deviceInfo.diskname + Const.Path.CustomMapsPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(deviceInfo.diskname + Const.Path.CustomMapsPath);
                    FilesWork.Folder.Clean(dir);
                    logs.Add(deviceInfo, Const.Log.DeviceMapsClean);
                }
                else
                {
                    deviceInfo.warning = true;
                    logs.Add(deviceInfo, Const.Log.Error.NoMapsFolder);
                }
            }
            catch
            {
                deviceInfo.warning = true;
                logs.Add(deviceInfo, Const.Log.Error.DeviceMapsClean);
            }
        }

        public void LoadMap() //download maps on device
        {
            try
            {
                if (File.Exists(Const.Path.CustomMaps + @"\" + Properties.Settings.Default.MapName))
                {
                    if (!Directory.Exists(deviceInfo.diskname + Const.Path.CustomMapsPath))
                        Directory.CreateDirectory(deviceInfo.diskname + Const.Path.CustomMapsPath);
                    File.Copy(Const.Path.CustomMaps + @"\" + Properties.Settings.Default.MapName,
                        deviceInfo.diskname + Const.Path.CustomMapsPath + @"\" + Properties.Settings.Default.MapName, true);
                    logs.Add(deviceInfo, Const.Log.DeviceMapLoad);
                }
                else
                {
                    deviceInfo.error = true;
                    if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                    logs.Add(deviceInfo, Const.Log.Error.NoMapFile);
                }
            }
            catch
            {
                deviceInfo.error = true;
                if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                logs.Add(deviceInfo, Const.Log.Error.DeviceMapLoad);
            }
        }

        public void DeleteDevice() //removing the device
        {
            deviceInfo.extract = true;
            if (deviceRenameWindow != null)
                if (deviceRenameWindow.IsVisible)
                    deviceRenameWindow.Close();
        }

        private void RenameDevice() //to rename the device
        {
            try
            {
                deviceRenameWindow = new WindowDeviceRename(deviceInfo.nickname);
                if (deviceRenameWindow.ShowDialog().Value)
                {
                    if (!deviceInfo.extract && File.Exists(deviceInfo.diskname + Const.Path.GarminXml)) 
                    {
                        string tempGarminXml = System.Windows.Forms.Application.StartupPath
                            + @"\" + System.IO.Path.GetFileName(Const.Path.GarminXml);
                        File.Copy(deviceInfo.diskname + Const.Path.GarminXml, tempGarminXml, true);
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(tempGarminXml);
                        XmlElement xRoot = xDoc.DocumentElement;
                        bool exits = false;
                        XmlElement xmlElement = null;
                        foreach (XmlElement xmlElementFind in xRoot)
                        {
                            if (xmlElementFind.Name == Const.Xml.Nickname)
                            {
                                xmlElement = xmlElementFind;
                                exits = true;
                                break;
                            }
                        }
                        string newLog;
                        if (deviceRenameWindow.nickname != null && deviceRenameWindow.nickname.Length > 0)
                        {
                            if (exits)
                            {
                                xmlElement.InnerText = deviceRenameWindow.nickname;
                                newLog = Const.Log.DeviceRename + deviceRenameWindow.nickname + ".";
                                LabelNickname.Content = deviceRenameWindow.nickname;
                            }
                            else
                            {
                                XmlElement xmlElem = xDoc.CreateElement(Const.Xml.Nickname, xRoot.NamespaceURI);
                                xmlElem.InnerText = deviceRenameWindow.nickname;
                                LabelNickname.Content = deviceRenameWindow.nickname;
                                xRoot.AppendChild(xmlElem);
                                newLog = Const.Log.DeviceNicknameAdd + deviceRenameWindow.nickname + ".";
                            }
                        }
                        else
                        {
                            if (exits) xRoot.RemoveChild(xmlElement);
                            LabelNickname.Content = Const.Label.NoNickname;
                            newLog = Const.Log.DeviceNicknameDelete;
                        }
                        xDoc.Save(tempGarminXml);
                        File.Copy(tempGarminXml, deviceInfo.diskname + Const.Path.GarminXml, true);
                        File.Delete(tempGarminXml);
                        logs.Add(deviceInfo, newLog);
                        deviceInfo.nickname = deviceRenameWindow.nickname;
                    }
                }
            }
            catch
            {
                if (Properties.Settings.Default.SoundError) Sound.Play(Const.Path.Sound.Error);
                logs.Add(deviceInfo, Const.Log.Error.RenameDevice);
            }
        }

        private void DataGridDeviceRename_Click(object sender, RoutedEventArgs e) 
        {
            if (!deviceInfo.extract) RenameDevice();
        }

        private void LabelNickname_MouseDoubleClick(object sender, MouseButtonEventArgs e) 
        {
            if (!deviceInfo.extract) RenameDevice();
        }

        private void DataGridDeviceAuto_Click(object sender, RoutedEventArgs e)
        {
            StartAuto();
        }

        private void DataGridDeviceFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", deviceInfo.diskname);
        }

        private void MenuItemTracksDownload_Click(object sender, RoutedEventArgs e)
        {
            CopyTracks();
        }

        private void MenuItemTracksClean_Click(object sender, RoutedEventArgs e)
        {
            CleanTracks();
        }

        private void MenuItemMapsClean_Click(object sender, RoutedEventArgs e)
        {
            CleanMaps();
        }

        private void MenuItemMapsLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadMap();
        }
    }
}
