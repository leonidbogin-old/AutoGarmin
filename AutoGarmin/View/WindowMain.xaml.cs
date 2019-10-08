using AutoGarmin.Class;
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
        private Devices devices;
        private Logs logs;
        #endregion

        #region USB
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

        private void UpdateDevices(bool first) //Обновление устройств (true = Первое обновление)
        {
            CheckFile(); //Проверка файла на актуальность
            devices.CheckStart(); //Начало проверки устройств на актуальность

            //Выборка букв USB устройств
            DriveInfo[] D = DriveInfo.GetDrives();
            foreach (DriveInfo DI in D)
            {
                if (DI.DriveType == DriveType.Removable)
                { 
                    //Проверка устройств на Garmin устройство
                    if (File.Exists(Convert.ToString(DI.Name) + Const.Path.GarminXml))
                    {
                        string id = null;
                        string model = null;
                        string nickname = null;

                        //Загрузка данных устройства из XML файла
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.Load(Convert.ToString(DI.Name) + Const.Path.GarminXml);
                        XmlElement xRoot = xDoc.DocumentElement;

                        foreach (XmlNode xnode in xRoot)
                        {
                            if (xnode.Name == Const.Xml.Id) id = xnode.InnerText;
                            else if (xnode.Name == Const.Xml.Model)
                            {
                                foreach (XmlNode xmodel in xnode)
                                {
                                    if (xmodel.Name == Const.Xml.Description) model = xmodel.InnerText;
                                }
                            }
                            else if (xnode.Name == Const.Xml.Nickname) nickname = xnode.InnerText;
                        }

                        if (id != null)
                            if (!devices.Check(id))
                            {
                                if (!first && Properties.Settings.Default.SoundConnect)
                                    Sound.Play(Const.Path.Sound.Connect);
                                devices.Add(id, nickname, Convert.ToString(DI.Name), model); //Добавление устройства
                            }
                    }
                }
            }
            devices.CheckEnd(); //Конец проверки устройств на актуальность (не актуальные удаляются)            
        }

        private void UpdateDevices() //Не первое обновление устройств
        {
            UpdateDevices(false);
        }
        #endregion

        #region Window
        public WindowMain() //Инициализация
        {
            InitializeComponent();

            logs = new Logs();
            logs.Visibility = Visibility.Hidden;
            GridContent.Children.Add(logs);

            devices = new Devices(ref logs);
            devices.Visibility = Visibility.Visible;
            GridContent.Children.Add(devices);            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) //Загрузка окна
        {
            LoadSidebar();
            //Hwnd start
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
            UpdateDevices(true);
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e) //Окно получило фокус
        {
            CheckFile();
            UpdateDevices();
        }
        #endregion

        #region MainButton
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
            if (!Directory.Exists(Const.Path.GPX)) Directory.CreateDirectory(Const.Path.GPX);
            Process Proc = new Process();
            Proc.StartInfo.FileName = "explorer";
            Proc.StartInfo.Arguments = Const.Path.GPX;
            Proc.Start();
            Proc.Close();
        }
        #endregion

        #region SideBar
        private void LoadSidebar() //Загрузка Sidebar
        {
            CheckFile(); //Проверка файла на актуальность

            CheckBoxTracksDownload.IsChecked = Properties.Settings.Default.AutoTracksDownload;
            CheckBoxTracksClear.IsChecked = Properties.Settings.Default.AutoTracksClean;
            CheckBoxMapClean.IsChecked = Properties.Settings.Default.AutoMapClean;
            CheckBoxMapLoad.IsChecked = Properties.Settings.Default.AutoMapLoad;
            CheckBoxAutoOn.IsChecked = Properties.Settings.Default.AutoOn;

            CheckBoxSoundConnect.IsChecked = Properties.Settings.Default.SoundConnect;
            CheckBoxSoundReady.IsChecked = Properties.Settings.Default.SoundReady;
            CheckBoxSoundError.IsChecked = Properties.Settings.Default.SoundError;
            CheckBoxSoundDisconnect.IsChecked = Properties.Settings.Default.SoundDisconnect;

            if (Properties.Settings.Default.AutoOn)
            {
                devices.AutoWork = true;
                ButtonAuto.Style = (Style)FindResource("ButtonAutoOn");
                ButtonAuto.Content = Const.Label.ButonAuto.On;
                this.Title = Const.Title.MainAutoOn;
            }
            else
            {
                devices.AutoWork = false;
                ButtonAuto.Style = (Style)FindResource("ButtonAutoOff");
                ButtonAuto.Content = Const.Label.ButonAuto.Off;
                this.Title = Const.Title.MainAutoOn;
            }
        }
        #endregion

        #region MapFile
        public void CheckFile() //Проверка актульности файла, отображение файла
        {
            if (File.Exists(Const.Path.CustomMaps + @"\" + Properties.Settings.Default.MapName))
            {
                TextBoxFile.ToolTip = "";
                TextBoxFile.Text = Properties.Settings.Default.MapName;
                System.IO.FileInfo file = new System.IO.FileInfo(Const.Path.CustomMaps + @"\" + Properties.Settings.Default.MapName);
                LabelFileSize.Content = Const.Label.FileSize + " " + BytesToString(file.Length);
                CheckBoxMapLoad.Style = (Style)FindResource("CheckBox");
            }
            else
            {
                TextBoxFile.ToolTip = Const.Error.LoadMapNotWork;
                TextBoxFile.Text = "";
                LabelFileSize.Content = Const.Error.NoMapFile;
                CheckBoxMapLoad.Style = (Style)FindResource("CheckBoxError");
            }
        }

        private void OpenDialogMapFileChange() //Открытие диалога выбора файла
        {
            //System.Windows.Forms.MessageBox.Show(explorer);
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.CheckFileExists = true;
            openFile.Filter = "Map files(*.kmz)|*.kmz|All files(*.*)|*.*";
            openFile.Multiselect = false;
            openFile.CheckPathExists = true;
            openFile.Title = Const.Title.ChooseFile;
            if (Directory.Exists(Properties.Settings.Default.MapOldPath))
                openFile.InitialDirectory = Properties.Settings.Default.MapOldPath;
            else openFile.InitialDirectory = "shell:MyComputerFolder";
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MapFileChange(openFile.FileName);
            }
            CheckFile();
        }

        private void Window_Drop(object sender, System.Windows.DragEventArgs e) //Перетягивание файла в окно
        {
            if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
                bool exitsKmz = false;
                foreach (string file in files)
                    if ((Path.GetExtension(file)).ToLower() == (Const.Path.MapFileExtension).ToLower())
                    {
                        MapFileChange(files[0]);
                        exitsKmz = true;
                        break;
                    }
                //Если выполнение кода дошло сюда, 
                //  следовательно в выбранных файлах не было файла с расширением .kmz
                if (exitsKmz)
                {
                    if (System.Windows.Forms.MessageBox.Show(Const.Error.NoKmz, Const.Error.WordWarning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)
                        == System.Windows.Forms.DialogResult.Yes)
                    {
                        MapFileChange(files[0]);
                    }
                }
            }
            CheckFile();
        }

        private void MapFileChange(string path) //Изменение файла
        {
            if (!Directory.Exists(Const.Path.CustomMaps)) Directory.CreateDirectory(Const.Path.CustomMaps);
            try
            {
                if (path != System.Windows.Forms.Application.StartupPath
                    + @"\" + Const.Path.CustomMaps + @"\" + Properties.Settings.Default.MapName)
                {
                    DirectoryInfo dir = new DirectoryInfo(Const.Path.CustomMaps);
                    DeviceControl.DeleteAll(dir);
                    File.Copy(path, Const.Path.CustomMaps + @"\" + Path.GetFileName(path), true);
                    Properties.Settings.Default.MapName = Path.GetFileName(path);
                    Properties.Settings.Default.MapOldPath = Path.GetDirectoryName(path);
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.MapOldPath = Path.GetDirectoryName(path);
                    Properties.Settings.Default.Save();
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show(Const.Error.Copy + ".",
                    Const.Error.WordError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MapFileClean() //Удаление файла
        {
            DirectoryInfo dir = new DirectoryInfo(Const.Path.CustomMaps);
            DeviceControl.DeleteAll(dir);
            CheckFile();
        }

        private static string BytesToString(long byteCount) //Строка размера файла
        {
            string[] suf = { "Байт", "Кб", "Мб", "Гб", "Тб", "Пб", "Эб" }; //
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }

        private void ButtonFileChange_Click(object sender, RoutedEventArgs e) //Нажатие кнопки "Выбрать" файл
        {
            OpenDialogMapFileChange();
        }

        private void TextBoxFile_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenDialogMapFileChange();
        }

        private void ButtonFileClean_Click(object sender, RoutedEventArgs e) //Нажатие кновки удаления файла
        {
            MapFileClean();
        } 

        private void ContextMenuFile_Open(object sender, RoutedEventArgs e) //Событие открытия ContextMenu файла
        {
            CheckFile();
        }
        #endregion

        #region AutoSettings
        private void CheckBoxTracksDownload_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoTracksDownload = true;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxTracksDownload_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoTracksDownload = false;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxTracksClear_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoTracksClean = true;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxTracksClear_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoTracksClean = false;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxMapClean_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoMapClean = true;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxMapClean_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoMapClean = false;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxMapLoad_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoMapLoad = true;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxMapLoad_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoMapLoad = false;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxAutoOn_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoOn = true;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxAutoOn_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoOn = false;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Audio
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

        private void CheckBoxSoundError_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SoundError = true;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxSoundError_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SoundError = false;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region ButtonAuto
        private void ButtonAuto_Click(object sender, RoutedEventArgs e)
        {
            if (devices.AutoWork)
            {
                devices.AutoWork = false;
                ButtonAuto.Style = (Style)FindResource("ButtonAutoOff");
                ButtonAuto.Content = Const.Label.ButonAuto.Off;
                this.Title = Const.Title.Main;
            }
            else
            {
                devices.AutoWork = true;
                ButtonAuto.Style = (Style)FindResource("ButtonAutoOn");
                ButtonAuto.Content = Const.Label.ButonAuto.On;
                this.Title = Const.Title.MainAutoOn;
                if (devices.devices.Count > 0)
                {
                    foreach (Device device in devices.devices)
                        if (!device.ready)
                        {
                            device.control.StartAuto();
                        }
                }
            }
        }
        #endregion

        private void DataGridContentUpdate_Click(object sender, RoutedEventArgs e) 
        {
            UpdateDevices();
        }
    }
}
