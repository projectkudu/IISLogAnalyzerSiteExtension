using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web;
using System.Runtime.Remoting;
using System.Diagnostics;
using WebAnalytics.Model.Metrics;
using Newtonsoft.Json;
using WebAnalytics.EngineLayer;
namespace WebAnalytics.Controllers
{
    public class AnalyticsController : ApiController
    {
        //string path = @"C:\Users\t-hawkf\Desktop\TempLogs";
        string path = @"C:\Users\t-hawkf\Desktop\Logs\W3SVC1";
        private AnalyticsEngine _analytics;

        public AnalyticsController()
        {
            //make an instance of the Anaylytics Engine
            _analytics = new AnalyticsEngine();
            _analytics.LogDirectory = path;
        }

        [HttpGet]
        //Analytics API routing: /diagnostics/analytics?{metrics=metricValues}&{start=datetime}&{end=datetime}&{interval=timeInterval}&{arguments= %7B%22{key}%22%3A%22{value}%22%7D}
        //When customers are using this API they have to append a URL encoded parameters for their metric computations to work
        //public Dictionary<string, List<KeyValuePair<string, object>>> GetAnalytics(String metrics, DateTime start, DateTime end, TimeSpan interval, string arguments)
        //public Dictionary<string, string> GetAnalytics(String metrics, DateTime start, DateTime end, TimeSpan interval, Dictionary<string,string> arguments)
        // Returns a JSON wrapping of the data
        public string GetAnalytics(String metrics, DateTime start, DateTime end, TimeSpan interval, string arguments)
        {
            Trace.WriteLine(end.ToString());
            //convert the JSON data into dictionary
            Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(arguments);
            
            if (metrics != null)
            {
                string[] requestedMetrics = metrics.Split(',');

                foreach (string requestedMetric in requestedMetrics)
                {
                    _analytics.AddMetricFactor(() => ActivateMetrics(requestedMetric,parameters));
            
                }
            }

            MetricResult result = _analytics.RunAlternativeEngine(start, end, interval);
            string jsonString = JsonConvert.SerializeObject(result);
            return jsonString;
        }

        [HttpGet]
        public List<MetricInfo> GetAvailableMetrics()
        {
            List<MetricInfo> metricInformation = _analytics.GetMetricsDescriptions();
            return metricInformation;
        }

        [HttpGet]
        public string GetMetricChartInformation(string metric)
        {
            //Get all the chart information for the given metric
            return "";
        }

        [NonAction]
        private IMetric ActivateMetrics(string metric, Dictionary<string,string> args)
        {
            ObjectHandle handle;
            try
            {
                Trace.WriteLine(typeof(IMetric).Assembly.ToString());
                handle = Activator.CreateInstance(typeof(IMetric).Assembly.FullName, "WebAnalytics.Model.Metrics." + metric);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return null;
            }
            
            //return the wrapped object
            IMetric m = (IMetric)handle.Unwrap();
            Trace.WriteLine(m.MetricName);
            try
            {
                //Send the arguments that was appended to the URI and let each metric strip the arguments of what it may need to compute its metric value.
                m.SetParameters(args);
            }
            catch (KeyNotFoundException)
            {
                throw new Exception("Metric cannot be computed without arguments");
            }
            catch (NotImplementedException)
            {
                //no op
            }
            
            return m;
        }

    }
}

