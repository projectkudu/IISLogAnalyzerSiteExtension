using Kudu.Core.LogHelper;
using System;
using System.Collections.Generic;
using WebAnalytics.Model.Logs;
using WebAnalytics.Models.BookmarkAPI;

namespace WebAnalytics.DataLayer
{
    public class DataEngine:IDataLayerAPI
    {
        private Dictionary<string, long> _logFiles;

        /// <summary>
        /// Set the location of the log files and create bookmarks for each of these log files.
        /// </summary>
        /// <param name="path">Directory that holds all the log files</param>
        public void SetDataEngine(string path)
        {
            
            
            AzureEnvironment e = new AzureEnvironment();
            _logFiles = LogServiceHelper.GetDirectoryFiles(e.LogFiles);
            //LogFileBookmarkManager bookmarkManager = new LogFileBookmarkManager();
            //bookmarkManager.RunManager(path);
        }

        /// <summary>
        /// Given a start and end date, return logs in a W3C_Extended object in that time interval
        /// Only return logs that have cookies within them
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>Lines of DeSerialized logs in a given time interval</returns>
        public IEnumerable<W3C_Extended_Log> GetLines(DateTime start, DateTime end)
        {
            //iterate through our directory of files
            foreach (string logFile in _logFiles.Keys)
            {
                LogParser logParser = new LogParser();
                logParser.LogFormat = W3C_ExtendedConstants.FORMAT;
                logParser.setTimes(start, end);

                //see that its capable to read this file
                logParser.FileName = logFile;


                //once the bookmarks are created for all our files

                if (!logParser.IsCapable)
                {
                    continue;
                }

                foreach (W3C_Extended_Log log in logParser.ParseW3CFormat())
                {
                    
                    if (log.UTCLogDateTime.CompareTo(start) >= 0 && log.UTCLogDateTime.CompareTo(end) < 0)
                    {
                        yield return log;
                    }
                }
            }
        }
    }
}
