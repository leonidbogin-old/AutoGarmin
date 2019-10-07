using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin
{
    public static class Const
    {
        public static class Title
        {
            public const string Main = "AutoGarmin 1.0";
            public const string MainAutoOn = Main + " - (Авто режим)";
            public const string ChooseFile = "Выбор файла для заливки";
            public const string RenameDevice = "Новое наименование устройства";
        }
        
        public static class Xml
        {
            public const string Nickname = "Nickname";
        }

        public static class Message
        {
            public const string DeviceConnect = "Устройство подключено";
            public const string DeviceDisconnect = "Устройство подключено";
        }

        public static class Label
        {
            public const string NoNickname = "Нет наименования";
            public const string FileSize = "Размер файла";

            public static class ButonAuto
            {
                public const string On = "Авто режим";
                public const string Off = "Включить";
            }
        }

        public static class Error
        {
            public const string NoKmz = "У выбранного файла расширение не .kmz, вы хотите продолжить?";
            public const string WordError = "Ошибка";
            public const string WordWarning = "Предупреждение";
            public const string Copy = "Ошибка при копировании файла карты";
            public const string LoadMapNotWork = "Заливка карты без файла не работает";
            public const string NoMapFile = "Нет файла карты";
        }

        public static class Path
        {
            public const string MapFileExtension = ".kmz";
            public const string CustomMaps = @"CustomMaps";
            public const string GPX = @"GPX";
            public const string GarminXml = @"\Garmin\GarminDevice.xml";
            public const string GarminIco = @"\Garmin\Garmintriangletm.ico";
            public const string NoIco = @"pack://application:,,,/no.ico";

            public static class Sound
            {
                public const string Connect = @"sounds/connect.wav";
                public const string Ready = @"sounds/ready.wav";
                public const string Disconnect = @"sounds/disconnect.wav";
            }
        }
    }
}
