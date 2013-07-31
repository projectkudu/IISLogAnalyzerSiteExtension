using System.Collections.Generic;
using WebAnalytics.Model.Logs;

namespace WebAnalytics.Model.Metrics
{
    public class AverageLatencyMetric:IMetric
    {
        private double total = 0;
        private int count = 0;
        public AverageLatencyMetric()
        {
        }
        public string MetricName { get { return "Average Time Taken"; } set { } }

        public void ProcessEntry(HttpLogEntry entry)
        {
            total += entry.TimeTaken;
            count++;
        }

        public object GetResult()
        {
            if (count == 0)
            {
                return 0;
            }
            return total / count;
        }

        public void SetParameters(Dictionary<string, string> args)
        {
            //no op
        }


        public string GetMetricDescription
        {
            get { return "Determine the average from logs timetaken field."; }
        }


        List<MetricParameter> IMetric.GetParameters()
        {
            return null;
        }
    }
}
