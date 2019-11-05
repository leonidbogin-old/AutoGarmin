using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin.Class
{
    public class DeviceInfo //device information
    {
        public string id { get; set; } //id device
        public string nickname { get; set; } //name
        public string model { get; set; } //model
        public string diskname { get; set; } //drive letter
        public string icon { get; set; }

        public DateTime timeConnect { get; set; }
    }
}
