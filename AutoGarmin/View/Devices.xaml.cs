using AutoGarmin.Class;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace AutoGarmin.View
{
    public partial class Devices : UserControl
    {
        private Logs logs; //link to log
        public bool AutoWork; //Status of operation of the program
        public List<Device> deviceList = new List<Device>(); //device list

        public Devices(ref Logs logs) //start
        {
            this.logs = logs; //savelink
            InitializeComponent();
        }

        public void CheckStart() //Start checking the relevance of devices
        {
            foreach (Device device in deviceList)
            {
                device.deviceInfo.check = true; //Mark all devices
            }
        }

        public bool Check(string id) //Check the device for relevance
        {
            foreach (Device device in deviceList)
            {
                if (device.deviceInfo.id == id)
                {
                    device.deviceInfo.check = false; //Remove the flag from the checked devices
                    return true;
                }
            }
            return false;
        }
        
        public void CheckEnd() //End of validity check. Remove out-of-date.
        {
            for (int i = 0; i < deviceList.Count; i++)
            {
                if (deviceList[i].deviceInfo.check)
                {
                    if (Properties.Settings.Default.SoundDisconnect)
                        Sound.Play(Const.Path.Sound.Disconnect);
                    StackPanelDevices.Children.Remove(deviceList[i]);
                    logs.Add(deviceList[i].deviceInfo, Const.Log.DeviceDisconnect);
                    //deviceList[i].deviceInfo = null;
                    deviceList[i].DeleteDevice();
                    deviceList.Remove(deviceList[i]);
                    i--;
                }
            }
        }
 
        public void Add(string id, string nickname, string diskname, string model) //Adding a deviceы
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
            Device device = new Device(deviceInfo, ref logs); 
            deviceList.Add(device);
            StackPanelDevices.Children.Add(device);
            logs.Add(deviceInfo, Const.Log.DeviceConnect);
            if (AutoWork)
            {
                //Start auto mode
                device.StartAuto();
            }
        }
    }
}
