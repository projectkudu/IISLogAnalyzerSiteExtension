﻿using System;
using System.Collections.Generic;

namespace WebAnalytics.Model.Logs
{
    public abstract class HttpLogEntry
    {
        /// <summary>
        /// All logs have some type of date information, extend this class to have your own format of how the date may be formatted
        /// Eg, YYYY-MM-DD, MM-DD-YY, MM/DD/YYYY, MM/DD/YY
        /// </summary>
        public abstract DateTime Date{get;set;}

        /// <summary>
        /// Time at which the activity occured
        /// </summary>
        public abstract DateTime Time{get;set;}

        public abstract DateTime UTCLogDateTime { get; set; }

        public abstract int BytesReceived{ get;set;}

        public abstract int BytesSent{ get; set;}

        public abstract int StatusCode{get;set;}

        public abstract string URIRequested{get;set;}

        public abstract Uri Referrer { get; set; }

        public abstract Dictionary<string,string> Cookies{get;set;}

        public abstract int TimeTaken{get;set;}

        public abstract string TypeRequest { get; set; }

        public abstract System.Net.IPAddress ClientIP {get; set;}

        public abstract System.Net.IPAddress ServerIP { get; set; }
    }
}
