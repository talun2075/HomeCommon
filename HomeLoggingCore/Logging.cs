using System;
using System.Collections.Generic;
using NLog;
using NLog.Targets;
using NLog.Config;
namespace HomeLogging
{
    public class Logging : ILogging
    {
        private Logger NLogLogger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Constructor with default settings
        /// </summary>
        public Logging()
        {
            //Default
            CreateLoggerConfig(new LoggerWrapperConfig() { ConfigName = "Sonos", ErrorFileName = "Errors.txt", TraceFileName = "Trace.txt", InfoFileName = "Battery.txt" });
        }
        /// <summary>
        /// Constructor with custom LoggerWrapperConfig
        /// </summary>
        /// <param name="lwc"></param>
        public Logging(LoggerWrapperConfig lwc)
        {
            if (string.IsNullOrEmpty(lwc.ConfigName))
            {
                lwc.ConfigName = "The ConfigName of the LoggerWrapperConfig can´t be empty";
                throw new Exception("The ConfigName of the LoggerWrapperConfig can´t be empty");
            }
            CreateLoggerConfig(lwc);
        }
        /// <summary>
        /// Internal Logging to Debug on Server.
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="message"></param>
        public void TraceLog(string Target, string message)
        {
            try
            {
                NLogLogger.Trace(Target + ":" + message);
            }
            catch (Exception ex)
            {
                ServerErrors.Add("Tracelog", ex.Message);
            }

        }
        /// <summary>
        /// Logs Info Messages.
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="message"></param>
        public void InfoLog(string Target, string message)
        {
            try
            {
                NLogLogger.Info(Target + ":" + message);
            }
            catch (Exception ex)
            {
                ServerErrors.Add("Tracelog", ex.Message);
            }

        }
        /// <summary>
        /// Write Exception to logg
        /// </summary>
        /// <param name="Method">NAme of the Method</param>
        /// <param name="ExceptionMes">Exception</param>
        /// <param name="_device">A optional Name for the log.</param>
        public void ServerErrorsAdd(string Method, Exception ExceptionMes, String _device = "")
        {
            try
            {
                String LogDevice = string.IsNullOrEmpty(_device) ? String.Empty : _device + ":";
                if (ExceptionMes == null || ExceptionMes.Message.StartsWith("Could not connect to device")) return;
                string error = LogDevice + Method;
                NLogLogger.Error(ExceptionMes, error);
            }
            catch (Exception ex)
            {
                ServerErrors.Add("ServerErrorsAdd", ex.Message);
            }
        }
        public void ServerErrorsAddWithStack(string Method, Exception ExceptionMes, String _device = "")
        {
            try
            {
                String LogDevice = string.IsNullOrEmpty(_device) ? String.Empty : _device + ":";
                if (ExceptionMes == null || ExceptionMes.Message.StartsWith("Could not connect to device")) return;
                string error = LogDevice + Method;
                NLogLogger.Error(ExceptionMes, error);
            }
            catch (Exception ex)
            {
                ServerErrors.Add("ServerErrorsAdd", ex.Message);
            }
        }
        /// <summary>
        /// Create the logger Config for this instance of LogginWrapper
        /// </summary>
        /// <param name="lwc"></param>
        private void CreateLoggerConfig(LoggerWrapperConfig lwc = null)
        {
            try
            {
                LoggingConfiguration config = LogManager.Configuration;
                if (config == null)
                {
                    config = new LoggingConfiguration();
                }
                string logpath = LoggingPath + @"\Logging\";
                if (!string.IsNullOrEmpty(lwc.PathToLogFiles))
                    logpath = lwc.PathToLogFiles;
                if (lwc.AddDateTimeToFilesNames)
                    logpath += DateTime.Now.ToString("yyyy-MM-dd") + "_";
                if (!string.IsNullOrEmpty(lwc.ErrorFileName))
                {
                    var logerrors = new FileTarget("logfileerror" + lwc.ConfigName) { FileName = logpath + lwc.ErrorFileName, Name = lwc.ConfigName };
                    logerrors.Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss} | ${level} | ${message} | Exception: ${exception} | ${newline} Stack: ${exception:format=tostring}";
                    config.AddTarget("logfileerror" + lwc.ConfigName, logerrors);
                    config.LoggingRules.Add(new LoggingRule(lwc.ConfigName, LogLevel.Error, LogLevel.Fatal, logerrors));
                }
                if (!string.IsNullOrEmpty(lwc.InfoFileName))
                {
                    var logfileInfo = new FileTarget("logfileinfo" + lwc.ConfigName) { FileName = logpath + lwc.InfoFileName, Name = lwc.ConfigName };
                    config.AddTarget("logfileinfo" + lwc.ConfigName, logfileInfo);
                    config.LoggingRules.Add(new LoggingRule(lwc.ConfigName, LogLevel.Info, LogLevel.Info, logfileInfo));
                }
                if (!string.IsNullOrEmpty(lwc.TraceFileName))
                {
                    var logfileTrace = new FileTarget("logfiletrace" + lwc.ConfigName) { FileName = logpath + lwc.TraceFileName, Name = lwc.ConfigName };
                    config.AddTarget("logfiletrace" + lwc.ConfigName, logfileTrace);
                    config.LoggingRules.Add(new LoggingRule(lwc.ConfigName, LogLevel.Trace, LogLevel.Trace, logfileTrace));
                }
                // Apply config
                LogManager.Configuration = config;
                NLogLogger = LogManager.GetLogger(lwc.ConfigName);
            }
            catch (Exception ex)
            {
                ServerErrors.Add("CreateLoggerConfig", ex.Message);
            }
        }
        /// <summary>
        /// LogginPath (Default BaseDirectory of Solution+\Logging\)
        /// If in the LoggingWraperConfig under PathToLogFiles a Path we will use this
        /// </summary>
        public String LoggingPath { get; private set; } = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// Loggin Dictionary on Errors in Loggin Case
        /// </summary>
        public Dictionary<String, String> ServerErrors { get; private set; } = new Dictionary<String, String>();

    }
}
