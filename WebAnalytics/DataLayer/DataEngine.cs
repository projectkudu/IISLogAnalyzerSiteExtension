﻿using Kudu.Core.LogHelper;
using System;
using System.Collections.Generic;
using WebAnalytics.Model.Logs;
using WebAnalytics.Models.BookmarkAPI;

namespace WebAnalytics.DataLayer
{
    public class DataEngine:IDataLayerAPI
    {
        private Dictionary<string, long> _logFiles;

        public void SetLogDirectory(string path)
        {
            _logFiles = LogServiceHelper.GetDirectoryFiles(path);
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
            LogParser logParser = new LogParser();
            LogFileBookmarkParser lfp = new LogFileBookmarkParser();
            logParser.LogFormat = W3C_ExtendedConstants.FORMAT;
            logParser.setTimes(start, end);
            //iterate through our directory of files
            //Trace.WriteLine(start.ToString());
            foreach (string logFile in _logFiles.Keys)
            {
                //see that its capable to read this file
                logParser.FileName = logFile;

                //parse the file and create bookmarks
                lfp.ParseFile(logFile);

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
