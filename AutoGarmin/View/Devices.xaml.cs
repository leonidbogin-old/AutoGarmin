using AutoGarmin.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoGarmin.View
{
    public partial class Devices : UserControl
    {
        #region Links
        private Logs logs;
        #endregion

        public bool AutoWork; //Статус работы программы

        public List<Device> devices = new List<Device>(); //Список устройств

        public Devices(ref Logs logs) //Инициализация
        {
            this.logs = logs;
            InitializeComponent();
        }

        #region Check devices
        public void CheckStart() //Старт проверки акуальности устройств
        {
            foreach (Device device in devices)
            {
                device.check = true; //Отмечаем все устройства
            }
        }

        public bool Check(string id) //Проверяем устройство
        {
            foreach (Device device in devices)
            {
                if (device.id == id)
                {
                    device.check = false; //Убираем флаг у проверенных устройств
                    return true;
                }
            }
            return false;
        }
        
        public void CheckEnd() //Конец проверки актульности. Не акуальные удаляем
        {
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].check)
                {
                    if (Properties.Settings.Default.SoundDisconnect)
                        Sound.Play(Const.Path.Sound.Disconnect);
                    StackPanelDevices.Children.Remove(devices[i].control);
                    logs.Add(devices[i], Const.Message.DeviceDisconnect);
                    devices[i].control = null;
                    devices.Remove(devices[i]);
                    i--;
                }
            }
        }
        #endregion

        #region Auto
        public void Auto(object _device)
        {
            Device device = (Device)_device;
            //Copy tracks
            if (Properties.Settings.Default.AutoTracksDownload)
            {
                if (Directory.Exists(device.diskname + Const.Path.GPXPath))
                {
                    string pathTarget = Const.Path.GPX + @"\" + DateTime.Now.ToShortDateString()
                        + @"\(" + DateTime.Now.ToString(Const.Time.Folder) + ")";
                    if (device.nickname != null && device.nickname.Length > 0)
                        pathTarget += $" - ({device.nickname})";
                    if (device.model != null && device.model.Length > 0)
                        pathTarget += $" - ({device.model})";
                    pathTarget += $" - ({device.id})";
                    DirectoryInfo dirSource = new DirectoryInfo(device.diskname + Const.Path.GPXPath);
                    DirectoryInfo dirTrarget = new DirectoryInfo(pathTarget);
                    CopyAll(dirSource, dirTrarget);
                }
                else
                {
                    Sound.Play(Const.Path.Sound.Error);
                    logs.Add(device, Const.Error.NoTrack);
                }
            }
            //Clean tracks
            if (Properties.Settings.Default.AutoTracksClean)
            {
                if (Directory.Exists(device.diskname + Const.Path.GPXPath))
                { 
                    DirectoryInfo dir = new DirectoryInfo(device.diskname + Const.Path.GPXPath);
                    DeleteAll(dir);
                }
            }
            //End
            device.ready = true;
            Sound.Play(Const.Path.Sound.Ready);
            this.Dispatcher.Invoke((System.Threading.ThreadStart)delegate {
                device.control.RectangleDeviceStatus.Fill = new SolidColorBrush(
                    Color.FromRgb(11, 89, 141));
            });
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target) //Копирование папки
        {
            // Если директория для копирования файлов не существует, то создаем ее
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Копируем все файлы в новую директорию
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(System.IO.Path.Combine(target.ToString(), fi.Name), true);
            }

            // Копируем рекурсивно все поддиректории
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                // Создаем новую поддиректорию в директории
                DirectoryInfo nextTargetSubDir =
                  target.CreateSubdirectory(diSourceSubDir.Name);
                // Опять вызываем функцию копирования
                // Рекурсия
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static void DeleteAll(DirectoryInfo source) //Копирование папки
        {
            foreach (FileInfo file in source.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                dir.Delete(true);
            }
        }
        #endregion

        public void Add(string id, string nickname, string diskname, string model) //Добавление устройства
        {
            Device device = new Device()
            {
                id = id,
                nickname = nickname,
                diskname = diskname,
                model = model,
                check = false,
                timeConnect = DateTime.Now
            };
            device.control = new DeviceControl(ref device, ref logs); 
            devices.Add(device);
            StackPanelDevices.Children.Add(device.control);
            logs.Add(device, Const.Message.DeviceConnect);
            if (AutoWork)
            {
                //Запуск авто режима
                System.Threading.Thread myThread = new System.Threading.Thread(
                    new System.Threading.ParameterizedThreadStart(Auto));
                myThread.Start(device);
            }
        }
    }
}
