using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAnalytics.Models.LogBookmarks;
using System.IO;
using System.Diagnostics;
using System.Text;
using Kudu.Core.LogHelper;

namespace WebAnalytics.Models.BookmarkAPI
{
    public class LogFileBookmarkManager
    {
        //keep track of the bookmarks
        private HashSet<LogFileBookmark> _bookmarkCollection;
        private const string _CACHE_FILE = "bookmarks.txt";
        private Dictionary<string, long> _logFiles;

        public void RunManager(string logPathDirectory)
        {
            _logFiles = LogServiceHelper.GetDirectoryFiles(logPathDirectory);

            //check to see if the cache file for bookmarks exist
            if (File.Exists(_CACHE_FILE))
            {
                //update the bookmarks file by checking the cache file for its latest bookmark and from there check to see
                //if there are new entries and then check for new files different than what is inside of the cache
                StreamReader sr = new StreamReader(_CACHE_FILE);
            }
            else
            {
                Trace.WriteLine(Environment.CurrentDirectory);
                //create the cache file and simply start parsing 
                File.Create(_CACHE_FILE);
                foreach (string logFilePath in _logFiles.Keys)
                {
                    ParseFile(logFilePath, 0);
                }

                //Cache the bookmarks made when parsing the file
            }
        }

        private void CacheBookmarks()
        {
            StreamWriter sw = new StreamWriter(_CACHE_FILE);
            string line;
            foreach (LogFileBookmark lfb in _bookmarkCollection)
            {
                line = lfb.ToString();
                sw.WriteLine(line);
            }
        }



        //Given the 
        private void ParseFile(string fileName, long seekPosition)
        {
            _bookmarkCollection = new HashSet<LogFileBookmark>();
            DateTime lastDate = DateTime.MinValue;
            DateTime temp = new DateTime();
            LogFileBookmark bookmark;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                long length = fileStream.Length;
                fileStream.Position = seekPosition;
                //just enough chars to get the datetime
                char[] line = new char[20];

                //use stringbuilder to concatenate the chars and make a string out of the scanned chars
                StringBuilder sb;
                Boolean markMidnight = false;
                //keep track of seeks position, and use offset to go back the length of chars to have the date
                long currentPosition, offset;
                while (fileStream.Position != length)
                {
                    //get the first 300 characters of the line
                    for (int i = 0; i < line.Length; i++)
                    {
                        line[i] = Convert.ToChar(fileStream.ReadByte());
                    }

                    //create a new string out of the string array
                    sb = new StringBuilder();
                    sb.Append(line);
                    //use DateTime.TryParse to do some validation
                    if (DateTime.TryParse(sb.ToString(), out temp))
                    {
                        //mark every hour. lastDate is initialized to DateTime.MinValue so its hour is zero. If the
                        //scanned time is midnight then the difference will be zero. Assuming the file is in order of datetime
                        //we dont want to miss midnight, being the first log entry.
                        if ((temp.Hour - lastDate.Hour) >= 1 || !markMidnight)
                        {
                            markMidnight = true;
                            //set the last date to be new time for next iteration
                            lastDate = temp;
                            bookmark = new LogFileBookmark();
                            
                            bookmark.StartDateTime = temp;

                            //substract the seek position by the number of bytes for 20 chars
                            currentPosition = fileStream.Position;
                            offset = currentPosition - line.Length;

                            //set the location of bytes where any user will begin reading for this date
                            bookmark.Location = offset;

                            //the file that this bookmark is associated with
                            bookmark.FilePath = fileName;

                            _bookmarkCollection.Add(bookmark);
                        }
                    }
                    //read rest of the line until new line is returned
                    while (fileStream.ReadByte() != 10)
                    {
                        //no op
                    }
                    sb.Remove(0, line.Length);
                }
            }

            //iterate through the bookmarkCollection and show all the hour logs
            foreach (LogFileBookmark bookmarkS in _bookmarkCollection)
            {
                Trace.WriteLine(bookmarkS.StartDateTime);
            }
        }


        private void TestCorrectPosition(FileStream fileStream, long current, long offset, StringBuilder sb, char[] line)
        {
            fileStream.Position = offset;
            //get the first 300 characters of the line
            for (int i = 0; i < line.Length; i++)
            {
                line[i] = Convert.ToChar(fileStream.ReadByte());
            }
            sb = new StringBuilder();
            sb.Append(line);
            Trace.WriteLine(sb.ToString());
        }

    }
}