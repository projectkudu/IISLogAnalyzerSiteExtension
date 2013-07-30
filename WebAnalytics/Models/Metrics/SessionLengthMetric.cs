using System;
using System.Collections.Generic;
using WebAnalytics.DataLayer;
using WebAnalytics.Model.Logs;
namespace WebAnalytics.Model.Metrics
{
    /// <summary>
    /// This metric calculates the session length of each individual sessions for users
    /// </summary>
    public class SessionLengthMetric:IMetric
    {
        private Dictionary<string, int> _uniqueSessionIDs = new Dictionary<string, int>();
        private Dictionary<string, Interval> _sessionLengths = new Dictionary<string, Interval>();
        private DateTime min = new DateTime(DateTime.MinValue.Ticks);

        public string MetricName { get { return "Session Length"; } set { } }

        //In this method we will peform computations to find session lengths
        public void PerformMetricJob(HttpLog resource)
        {
            Dictionary<string, string> cookies = resource.Cookies;
            if (cookies != null)
            {
                string sessionID = "";
                if (cookies.TryGetValue(CookieParser.WA_WEBSITE_SID, out sessionID))
                {
                    Organize(sessionID, resource.UTCLogDateTime);
                }
            }
        }

        class Interval
        {
            public DateTime startTime;
            public DateTime endTime;
        }

        private bool AddSession(string sessionID)
        {
            if (!_uniqueSessionIDs.ContainsKey(sessionID))
            {
                _uniqueSessionIDs.Add(sessionID, 1);
                return false;
            }
            return true;
        }

        private void Organize(string arg, DateTime time)
        {
            Interval interval = new Interval();
            //if the key does not exist then it was added to the dictionary keep track of this session ids startime
            if (!AddSession(arg))
            {
                //if the session Id was unique then track the start time
                interval.startTime = time;

                //make the endtime same time as startTime in case this session id bounces on the site. This means session duration would be zero
                interval.endTime = interval.startTime;
                _sessionLengths.Add(arg, interval);
            }
            else //if its already in the unique dictionary, then this must be the second or more'th time to read this session id
            {
                _sessionLengths[arg].endTime = time;
            }
        }

        public object GetResult()
        {
            TimeSpan difference;
            double total = 0;
            foreach(KeyValuePair<string, Interval> pair in _sessionLengths)
            {
                difference = pair.Value.endTime - pair.Value.startTime;
                total += difference.TotalMinutes;
            }
            if (_uniqueSessionIDs.Count != 0)
            {

                double result = total / _uniqueSessionIDs.Count;
                return result;
            }
            return 0;
        }

        public void SetParameters(Dictionary<string, string> args)
        {
            //no op
        }


        public string GetMetricDescription
        {

            get { return "Find the length of time that each user was on this website per session."; }
        }


        List<MetricParameter> IMetric.GetParameters()
        {
            return null;
        }
    }
}
