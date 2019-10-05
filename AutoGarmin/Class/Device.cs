using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin
{
    public class Device
    {
        public UserControlDevice userControl { get; set; }
        public string id { get; set; }
        public string nickname { get; set; }
        public string diskname { get; set; }
        public string model { get; set; }
        public bool ready { get; set; }
        public bool check { get; set; }
        public DateTime timeConnect { get; set; }
    }
}
