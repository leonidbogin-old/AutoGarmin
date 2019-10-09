using System;
using System.Globalization;
using System.Windows.Data;

namespace AutoGarmin.Class
{
    class ActionToColorConverter : IValueConverter //Painting logs
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                case Const.Log.DeviceConnect:
                case Const.Log.DeviceDisconnect: return Const.Color.DeviceConnect();


                case Const.Log.Error.RenameDevice:
                case Const.Log.Error.DeviceTracksDownload:
                case Const.Log.Error.NoTracksFolder:
                case Const.Log.Error.NoMapFile:
                case Const.Log.Error.DeviceMapLoad: return Const.Color.Error();

                case Const.Log.Error.NoTracksFolderClean:
                case Const.Log.Error.NoMapsFolder:
                case Const.Log.Error.DeviceTracksClean:
                case Const.Log.Error.DeviceMapsClean: return Const.Color.Warning();

                case Const.Log.DeviceStartAuto: return Const.Color.DeviceStartAuto();

                case Const.Log.DeviceEndAuto: return Const.Color.DeviceReady();

                default: return Const.Color.Transparent();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
