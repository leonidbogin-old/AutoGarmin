namespace AutoGarmin
{
    public static class USB //working with usb
    {
        public static class Code //usb code
        {
            public const int WM_DEVICECHANGE = 0x0219; //something to do with usb
            public const int DBT_DEVICEARRIVAL = 0x8000; //the device is connected
            public const int DBT_DEVICEREMOVECOMPLETE = 0x8004; //the device is disconnected
        }
    }
}
