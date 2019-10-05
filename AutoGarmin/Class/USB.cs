using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin
{
    public static class USB
    {
        public static class Code
        {
            public const int WM_DEVICECHANGE = 0x0219; //что-то связанное с usb
            public const int DBT_DEVICEARRIVAL = 0x8000; //устройство подключено
            public const int DBT_DEVICEREMOVECOMPLETE = 0x8004; //устройство отключено
        }
    }
}
