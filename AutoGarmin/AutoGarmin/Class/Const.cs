using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin.Class
{
    public static class Const //Constants of the program
    {
        public static class Xml //Xml
        {
            public const string Id = "Id"; //id device
            public const string Model = "Model"; //Data
            public const string Description = "Description"; //Model
            public const string Nickname = "Nickname"; //name
            public const string GarminInformation = "GarminInformation"; //name
        }

        public static class Path //Work with files
        {
            public const string GarminXml = @"\Garmin\GarminDevice.xml";
            public const string GarminAutorun = @"autorun.inf";
            public const string GarminAutorunImage = @"icon=";
            public const string GarminInformationXml = @"GarminInformation.xml";

            public const string CustomMaps = @"CustomMaps";
            public const string CustomMapsPath = @"Garmin\" + CustomMaps;

            public const string NoIco = @"pack://application:,,,/no.ico";
            public const string Icon = @"pack://application:,,,/icon.ico";
        }

        public static class Time //Time format
        {
            public const string Connect = "HH:mm:ss";
        }

        public static class Label
        {
            public const string NoNickname = "Нет наименования";
            public const string NoNicknameTooltip = "Двойной клик, чтобы установить наименование";
        }
    }
}
