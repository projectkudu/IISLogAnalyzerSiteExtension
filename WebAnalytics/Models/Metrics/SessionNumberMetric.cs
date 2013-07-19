using System;
using System.Collections.Generic;
using WebAnalytics.DataLayer;
using WebAnalytics.Model.Logs;

namespace WebAnalytics.Model.Metrics
{
    public class SessionNumberMetric:IMetric
    {
        private Dictionary<string, int> _uniqueSessionIds = new Dictionary<string, int>();

        public SessionNumberMetric()
        {
        }

        public string MetricName { get { return "Number of Sessions"; } set { } }


        //the requirements are data fields that are needed from the log files 
        //public List<AnalyticsDataLayer.LogFields> MetricRequirements { get; set; }

        /// <summary>
        /// At the api controller level, time is already considered, now with the filtered logs dependent on the time the user specefied, perform the metric jobs
        /// for these log entries
        /// </summary>
        /// <param name="resource"></param>
        /// 
        public void PerformMetricJob(HttpLog resource)
        {
            if (resource.Cookies == null || resource.Cookies.Count == 0)
            {
                return;
            }
            //http://localhost:12553/diagnostics/analytics/getsessioncount?startTime=06/19/2013&endTime=06/20/2013&timeInterval=1:00
            //be sure to check if the cookies are empty. Some log files may not have cookies
            Dictionary<string,string> cookies = resource.Cookies;
            if (cookies != null)
            {
                string sessionID = "";
                if(cookies.TryGetValue(CookieParser.D4DAD, out sessionID))
                {
                    AddSession(sessionID);
                }
            }
        }

        private void AddSession(string arg)
        {
            if (!_uniqueSessionIds.ContainsKey(arg))
            {
                _uniqueSessionIds.Add(arg, 1);
            }
        }

        /// <summary>
        /// Return the number of unique sessions
        /// </summary>
        /// <returns></returns>
        public object GetResult()
        {
            return _uniqueSessionIds.Count;
        }


        public void SetParameters(string args)
        {
            throw new NotImplementedException();
        }


        public void SetParameters(Dictionary<string, string> args)
        {
            //no op
        }



        public string GetMetricDescription
        {
            get { return "Find the number of unique sessions."; }
        }

        List<MetricParameter> IMetric.GetParameters()
        {
            return null;
        }
    }
}
