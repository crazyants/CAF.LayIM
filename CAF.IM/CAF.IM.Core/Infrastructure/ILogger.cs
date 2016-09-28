using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.IM.Core.Infrastructure
{
    public interface ILogger
    {
        void Log(LogType type, string message);
    }
}