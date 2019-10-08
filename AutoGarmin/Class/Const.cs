using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin
{
    public static class Const
    {
        public static class Title //Заголовки окон
        {
            public const string Main = "AutoGarmin 1.0";
            public const string MainAutoOn = Main + " - (Авто режим)";
            public const string ChooseFile = "Выбор файла для заливки";
            public const string RenameDevice = "Новое наименование устройства";
        }
        
        public static class Xml //Xml константы
        {
            public const string Id = "Id"; //Уникальный id устройства
            public const string Model = "Model"; //Данные устройства
            public const string Description = "Description"; //Модель устройства
            public const string Nickname = "Nickname"; //Название поле, с помощью которого именуется устройство
        }

        public static class Message //Сообщения для логов
        {
            public const string DeviceConnect = "Устройство подключено.";
            public const string DeviceDisconnect = "Устройство отключено.";
            public const string DeviceTracksDownload = "Скачаны треки.";
            public const string DeviceTracksClean = "Очищена папка с треками.";
        }

        public static class Label //Текст для интерфейса
        {
            public const string NoNickname = "Нет наименования";
            public const string FileSize = "Размер файла";

            public static class ButonAuto //Кнопка включения авто режима
            {
                public const string On = "Авто режим";
                public const string Off = "Включить";
            }
        }

        public static class Error //Сообщения об ошибках
        {
            public const string NoKmz = "У выбранного файла расширение не .kmz, вы хотите продолжить?";
            public const string WordError = "Ошибка";
            public const string WordWarning = "Предупреждение";
            public const string Copy = "Ошибка при копировании файла карты";
            public const string LoadMapNotWork = "Заливка карты без файла не работает";
            public const string NoMapFile = "Нет файла карты";
            public const string NoTrack = "На устройстве отсуствует папка треков. Треки не были скачаны.";
        }

        public static class Path //Работа с папками, файлами и звуками
        {
            public const string MapFileExtension = ".kmz";
            public const string CustomMaps = @"CustomMaps";
            public const string GPX = @"GPX";
            public const string GPXPath = @"\Garmin\" + GPX;
            public const string GarminXml = @"\Garmin\GarminDevice.xml";
            public const string GarminIco = @"\Garmin\Garmintriangletm.ico";
            public const string NoIco = @"pack://application:,,,/no.ico";

            public static class Sound //Работа со звуками
            {
                public const string Connect = @"sounds/connect.wav";
                public const string Ready = @"sounds/ready.wav";
                public const string Error = @"sounds/error.wav";
                public const string Disconnect = @"sounds/disconnect.wav";
            }
        }

        public static class Time //Форматы времени
        {
            public const string Connect = "HH:mm:ss";
            public const string Log = "HH:mm:ss";
            public const string Folder = "HH-mm-ss";
        }
    }
}
