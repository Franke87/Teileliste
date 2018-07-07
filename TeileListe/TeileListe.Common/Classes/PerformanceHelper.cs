using System;
using System.Diagnostics;
using System.IO;

namespace TeileListe.Common.Classes
{
    public static class PerformanceHelper
    {
        public static void Messung(Action action, string beschreibung, int wiederholungen)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            action();

            stopwatch.Stop();

            File.AppendAllText("log.txt",
                                string.Format("{0} s and {1} ms für {2} Wiederholungen von {3}{4}",
                                stopwatch.Elapsed.Seconds,
                                stopwatch.Elapsed.Milliseconds,
                                wiederholungen,
                                beschreibung,
                                Environment.NewLine));
        }
    }
}
