using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAnalytics.Models.BookmarkAPI;

namespace WebAnalytics.Models.LogBookmarks
{
    public class LogFileBookmark:Bookmark
    {
        public string FilePath { get; set; }
        public DateTime StartDateTime { get; set; }
        public long Location { get; set; }
        public object Mark
        {
            get { throw new NotImplementedException(); }
        }
    }
}