using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin
{
    public class Devices
    {
        public class Device
        {
            public string id { get; set; }
            public string nickname { get; set; }
            public string diskname { get; set; }
            public string model { get; set; }
            public DateTime time_connect { get; set; }
            public bool ready { get; set; }
        }
    }
}
