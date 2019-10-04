using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGarmin
{
    public class Logs
    {
        public ObservableCollection<LogLine> logLines = new ObservableCollection<LogLine>();

        public void Add(string nickname, string diskname, string model, string action)
        {
            LogLine logNew = new LogLine()
            {
                time = DateTime.Now.ToString("HH:mm:ss"),
                nickname = nickname,
                diskname = diskname,
                model = model,
                action = action
            };
            logLines.Add(logNew);
        }

        public LogLine Last()
        {
            return logLines.Last();
        }

        public void Clear()
        {
            logLines.Clear();
        }

        public class LogLine
        {
            public string time { get; set; }
            public string nickname { get; set; }
            public string diskname { get; set; }
            public string model { get; set; }
            public string action { get; set; }
        }
    }
}
