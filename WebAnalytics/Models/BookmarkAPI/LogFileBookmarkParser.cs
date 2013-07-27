using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAnalytics.Models.LogBookmarks;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace WebAnalytics.Models.BookmarkAPI
{
    public class LogFileBookmarkParser
    {
        //keep track of the bookmarks
        private HashSet<LogFileBookmark> _bookmarkCollection;

        public void ParseFile(string fileName)
        {
            _bookmarkCollection = new HashSet<LogFileBookmark>();
            DateTime lastDate = new DateTime();
            DateTime temp = new DateTime();
            LogFileBookmark bookmark;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                //just enough chars to get the datetime
                char[] line = new char[20];
                StringBuilder sb;
                String date;
                long currentPosition, offset;
                int lineChar = 0;
                Boolean markedFirstDate = false;
                while (lineChar != -1)
                {
                    //get the first 300 characters of the line
                    for (int i = 0; i < line.Length; i++)
                    {
                        line[i] = Convert.ToChar(fileStream.ReadByte());
                    }

                    //create a new string out of the string array
                    sb = new StringBuilder();
                    sb.Append(line);
                    Trace.WriteLine(sb.ToString());
                    Trace.WriteLine(new TimeSpan(1, 0, 0).ToString());
                    //use DateTime.TryParse to do some validation
                    if (DateTime.TryParse(sb.ToString(), out temp))
                    {
                        if ((temp - lastDate).Ticks > new TimeSpan(1, 0, 0).Ticks)
                        {
                            lastDate = temp;
                            bookmark = new LogFileBookmark();
                            bookmark.StartDateTime = temp;

                            //substract the seek position by the number of bytes for 20 chars
                            currentPosition = fileStream.Position;
                            offset = currentPosition - line.Length;

                            //set the location of bytes where any user will begin reading for this date
                            bookmark.Location = offset;

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