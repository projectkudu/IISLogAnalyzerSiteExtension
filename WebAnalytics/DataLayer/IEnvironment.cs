using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAnalytics.DataLayer
{
    public interface IEnvironment
    {
        string LogFiles { get; }
    }
}