using System;
using System.Collections.Generic;
using WebAnalytics.Model.Logs;

namespace WebAnalytics.Model.Metrics
{
    public class StatusCodeMetric:IMetric
    {
        //Key to search for within the dictionary object
        private const string STATUS_CODE_KEY = "statuscode";
        private int desiredStatusCode;

        public StatusCodeMetric()
        {
        }
        private int _numStatus = 0;
        public string MetricName { get { return "Status Codes"; } set { } }

        public void PerformMetricJob(HttpLog resource)
        {
            //check the status code of the log
            if (resource.StatusCode == desiredStatusCode)
            {
                _numStatus++;
            }
        }

        public object GetResult()
        {
            return _numStatus;
        }

        /// <summary>
        /// Look for the keys needed in this metric and obtain the value, if not exist throw an exception
        /// </summary>
        /// <param name="args"></param>
        public void SetParameters(Dictionary<string, string> args)
        {
            string value;
            if (!args.TryGetValue(STATUS_CODE_KEY, out value))
            {
                throw new KeyNotFoundException("Could not find: " + STATUS_CODE_KEY);
            }
            else
            {
                desiredStatusCode = Convert.ToInt32(value);
            }
        }


        public string GetMetricDescription
        {
            get { return "Count how many occurences of a given status code"; }
        }

        List<MetricParameter> IMetric.GetParameters()
        {
            List<MetricParameter> parameters = new List<MetricParameter>();
            string defaultVal = Convert.ToString(200);
            var temp = new MetricParameter(STATUS_CODE_KEY, "Provide a status code number that you are interested in getting information on.", defaultVal);
            parameters.Add(temp);
            return parameters;
        }

        public string TypeOFGraph
        {
            get { return "line"; }
        }
    }
}
