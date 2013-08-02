using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Diagnostics;

namespace WebAnalytics.DataLayer
{
    public class AzureEnvironment:IEnvironment
    {
        private string _siteRootPath;
        private string _root;
        private string _logFilesPath;
        private string _rawLogsPath;
        private const string UserSite = "/_app";
        public AzureEnvironment()
        {
            _siteRootPath = HostingEnvironment.MapPath(UserSite);
            //_siteRootPath = @"C:\Users\t-hawkf\Desktop\GitDir\kudu\apps\analytics\site";
            //get the root of the project and map the root against desired directories such as logfiles
            _root = Path.GetFullPath(Path.Combine(_siteRootPath, ".."));
            _logFilesPath = Path.Combine(_root, "LogFiles");
            _rawLogsPath = Path.Combine(_logFilesPath, "http");
        }

        public string LogFiles
        {
            get { return _logFilesPath; }
        }

        public string HTTPLogs
        {
            get { return _rawLogsPath; }
        }
    }
}