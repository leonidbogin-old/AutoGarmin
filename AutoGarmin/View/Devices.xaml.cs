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
                device.control.StartAuto();
            }
        }
    }
}
