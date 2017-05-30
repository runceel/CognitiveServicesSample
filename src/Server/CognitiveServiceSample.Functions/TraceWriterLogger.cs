using CognitiveServicesSample.Commons;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognitiveServiceSample.Functions
{
    public class TraceWriterLogger : ILogger
    {
        private TraceWriter TraceWriter { get; }

        public TraceWriterLogger(TraceWriter traceWriter)
        {
            this.TraceWriter = traceWriter;
        }
        public void Error(string message)
        {
            this.TraceWriter.Error(message);
        }

        public void Error(string message, Exception ex)
        {
            this.TraceWriter.Error(message, ex);
        }

        public void Info(string message)
        {
            this.TraceWriter.Info(message);
        }

        public void Warn(string message)
        {
            this.TraceWriter.Warning(message);
        }
    }
}