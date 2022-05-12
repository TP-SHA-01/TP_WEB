using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.Log
{
    public class LogHelper
    {
        public static readonly Logger Logger;
        static LogHelper()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = "NLog/${shortdate}/${level}_${shortdate}.log",
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.DateAndSequence,
                ArchiveEvery = NLog.Targets.FileArchivePeriod.Day,
            };

            //var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets
            //config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config
            LogManager.Configuration = config;

            Logger = LogManager.GetCurrentClassLogger();
        }
        public static void Debug(string msg)
        {
            Logger.Debug(msg);
        }
        public static void Error(string msg)
        {
            Logger.Error(msg);
        }
        public static void Error(Exception ex)
        {
            Logger.Error(ex);
        }

        public static void Fatal(string message)
        {
            Logger.Fatal(message);
        }
    }
}
