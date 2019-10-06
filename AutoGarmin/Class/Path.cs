using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin
{
    public static class Path
    {
        public static string WindowTitle = "AutoGarmin 1.0";
        public static string WindowTitleAutoOn = WindowTitle + " - (Авто режим)";

        public const string GPX = @"GPX";
        public const string GarminXml = @"\Garmin\GarminDevice.xml";
        public const string GarminIco = @"\Garmin\Garmintriangletm.ico";
        public const string NoIco = @"pack://application:,,,/no.ico";

        public static class ButonAuto
        {
            public const string On = "Авто режим";
            public const string Off = "Включить";
        }

        public static class Sound
        {
            public const string Connect = @"sounds/connect.wav";
            public const string Ready = @"sounds/ready.wav";
            public const string Disconnect = @"sounds/disconnect.wav";
        }
    }
}
