using System;
using System.Collections.Generic;

namespace HomeLogging
{
    public interface ILogging
    {
        string LoggingPath { get; }
        Dictionary<string, string> ServerErrors { get; }

        void InfoLog(string Target, string message);
        void ServerErrorsAdd(string Method, Exception ExceptionMes, string _device = "");
        void TraceLog(string Target, string message);
    }
}