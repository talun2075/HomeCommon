using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeLogging
{
    /// <summary>
    /// LoggingWrapperConfigurations class.
    /// </summary>
    public class LoggerWrapperConfig
    {
        /// <summary>
        /// Path to Logfiles. If Empty it use the Fallback from the Loggingwrapper
        /// </summary>
        public String PathToLogFiles { get; set; }
        /// <summary>
        /// File Name for Error Logging
        /// </summary>
        public String ErrorFileName { get; set; } = "errors.txt";
        /// <summary>
        /// File Name for Info Logging
        /// </summary>
        public String InfoFileName { get; set; } = "info.txt";
        /// <summary>
        /// File Name for Trace Logging
        /// </summary>
        public String TraceFileName { get; set; } = "trace.txt";
        /// <summary>
        /// Important need to be set. This is the InstanceName for the LoggingMachine.
        /// </summary>
        public String ConfigName { get; set; }

        public Boolean AddDateTimeToFilesNames { get; set; } = true;
    }
}
