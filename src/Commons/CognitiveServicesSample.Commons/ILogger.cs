using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServicesSample.Commons
{
    public interface ILogger
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Error(string message, Exception ex);
    }
}
