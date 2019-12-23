using System;
using System.IO;

namespace EffortEstimator.Helpers
{
    public class Logger
    {
        public static void Log(string msg)
        {
            using StreamWriter w = File.AppendText("log.txt");
            w.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " - " + msg);
        }
    }
}
