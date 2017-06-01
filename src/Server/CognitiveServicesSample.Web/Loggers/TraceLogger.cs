using CognitiveServicesSample.Commons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace CognitiveServicesSample.Web.Loggers
{
    public class TraceLogger : ILogger
    {
        public void Error(string message)
        {
            Trace.TraceError(message);
        }

        public void Error(string message, Exception ex)
        {
            Trace.TraceError($"{message}:{ex}");
        }

        public void Info(string message)
        {
            Trace.TraceInformation(message);
        }

        public void Warn(string message)
        {
            Trace.TraceWarning(message);
        }
    }
}