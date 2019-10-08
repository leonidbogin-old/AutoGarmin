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

        public List<Device> devicesList = new List<Device>();

        public Devices(ref Logs logs) //Инициализация
        {
            this.logs = logs;
            InitializeComponent();
        }

        #region Check devicesList
        public void CheckStart() //Старт проверки акуальности устройств
        {
            foreach (Device device in devicesList)
            {
                device.deviceInfo.check = true; //Отмечаем все устройства
            }
        }

        public bool Check(string id) //Проверяем устройство
        {
            foreach (Device device in devicesList)
            {
                if (device.deviceInfo.id == id)
                {
                    device.deviceInfo.check = false; //Убираем флаг у проверенных устройств
                    return true;
                }
            }
            return false;
        }
        
        public void CheckEnd() //Конец проверки актульности. Не акуальные удаляем
        {
            for (int i = 0; i < devicesList.Count; i++)
            {
                if (devicesList[i].deviceInfo.check)
                {
                    if (Properties.Settings.Default.SoundDisconnect)
                        Sound.Play(Const.Path.Sound.Disconnect);
                    StackPanelDevices.Children.Remove(devicesList[i]);
                    logs.Add(devicesList[i].deviceInfo, Const.Log.DeviceDisconnect);
                    //devicesList[i].deviceInfo = null;
                    devicesList[i].DeleteDevice();
                    devicesList.Remove(devicesList[i]);
                    i--;
                }
            }
        }
        #endregion

        public void Add(string id, string nickname, string diskname, string model) //Добавление устройства
        {
            DeviceInfo deviceInfo = new DeviceInfo()
            {
                id = id,
                nickname = nickname,
                diskname = diskname,
                model = model,
                check = false,
                timeConnect = DateTime.Now
            };
            Device device = new Device(ref deviceInfo, ref logs); 
            devicesList.Add(device);
            StackPanelDevices.Children.Add(device);
            logs.Add(deviceInfo, Const.Log.DeviceConnect);
            if (AutoWork)
            {
                //Запуск авто режима
                device.StartAuto();
            }
        }
    }
}
