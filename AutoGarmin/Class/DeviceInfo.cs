using System;

namespace AutoGarmin
{
    public class DeviceInfo //device information
    {
        public bool check { get; set; } //checking for relevance
        public string id { get; set; } //id device
        public string nickname { get; set; } //name
        public string diskname { get; set; } //drive letter
        public string model { get; set; } //model
        public DateTime timeConnect { get; set; } //сonnection time
        public bool error { get; set; } //there is a mistake
        public bool warning { get; set; } //there is a warning
        public bool ready { get; set; } //the device is ready
        public bool extract { get; set; } //the device is removed
    }
}
