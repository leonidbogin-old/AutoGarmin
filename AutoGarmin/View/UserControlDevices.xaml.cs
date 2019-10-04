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
        //Ссылка на логи
        private UserControlLogs userControlLogs;

        public List<Device> devices = new List<Device>();

        public void CheckStart()
        {
            foreach (Device device in devices)
            {
                device.check = true;
            }
        }

        public bool Check(string id)
        {
            foreach (Device device in devices)
            {
                if (device.id == id)
                {
                    device.check = false;
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
                    UserControlDevice userControlDeviceRemove = null;
                    foreach (UserControlDevice userControlDevice in StackPanelDevices.Children)
                    {
                        if (userControlDevice.Name == "userControlDevice" + devices[i].id)
                            userControlDeviceRemove = userControlDevice;
                    }
                    StackPanelDevices.Children.Remove(userControlDeviceRemove);
                    userControlLogs.LogAdd(devices[i].id, devices[i].nickname,
                        devices[i].diskname, devices[i].model, "Устройство отключено");
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
                time_connect = DateTime.Now,
                check = false
            };
            devices.Add(device);
            UserControlDevice userControlDevice = new UserControlDevice("userControlDevice" + id, device);
            userControlDevice.ApplyTemplate();
            StackPanelDevices.Children.Add(userControlDevice);
            userControlLogs.LogAdd(device.id, device.nickname,
                        device.diskname, device.model, "Устройство подключено");
        }

        public UserControlDevices(UserControlLogs userControlLogs)
        {
            this.userControlLogs = userControlLogs;
            InitializeComponent();
        }

        public class Device
        {
            public string id { get; set; }
            public string nickname { get; set; }
            public string diskname { get; set; }
            public string model { get; set; }
            public DateTime time_connect { get; set; }
            public bool ready { get; set; }
            public bool check { get; set; }
        }
    }
}
