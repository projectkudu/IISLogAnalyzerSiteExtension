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
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string Location { get; set; }
        public object Mark
        {
            get { throw new NotImplementedException(); }
        }
    }
}