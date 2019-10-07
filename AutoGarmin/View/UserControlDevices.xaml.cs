using AutoGarmin.Class;
using System;
using System.Collections.Generic;
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
    public partial class UserControlDevices : UserControl
    {
        //Ссылка 
        private UserControlLogs logs;

        public List<Device> devices = new List<Device>();

        public void CheckStart()
        {
            foreach (Device device in devices)
            {
                device.check = true; //Отмечаем все устройства для проверки актуальности
            }
        }

        public bool Check(string id)
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
        
        public void CheckEnd()
        {
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].check)
                {
                    if (Properties.Settings.Default.SoundDisconnect)
                        Sound.Play(Const.Path.Sound.Disconnect);
                    StackPanelDevices.Children.Remove(devices[i].userControl);
                    logs.LogAdd(devices[i], Const.Message.DeviceDisconnect);
                    devices[i].userControl = null;
                    devices.Remove(devices[i]);
                    i--;
                }
            }
        }

        public void Add(string id, string nickname, string diskname, string model)
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
            device.userControl = new UserControlDevice(ref device, ref logs); 
            devices.Add(device);
            StackPanelDevices.Children.Add(device.userControl);
            logs.LogAdd(device, Const.Message.DeviceConnect);
        }

        public UserControlDevices(ref UserControlLogs logs)
        {
            this.logs = logs;
            InitializeComponent();
        }
    }
}
