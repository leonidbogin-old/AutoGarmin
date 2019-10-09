namespace AutoGarmin
{
    public class LogLine //log string
    {
        public string time { get; set; } //action time
        public string id { get; set; } //id device
        public string nickname { get; set; } //name
        public string modelAndDiskname { get; set; } //model and drive letter
        public string action { get; set; } //a string describing the action
    }
}
