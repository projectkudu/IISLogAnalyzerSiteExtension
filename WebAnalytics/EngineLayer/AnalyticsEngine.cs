using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using WebAnalytics.DataLayer;
using WebAnalytics.Model.Logs;
using WebAnalytics.Model.Metrics;
using WebAnalytics.Models.BookmarkAPI;

namespace WebAnalytics.EngineLayer
{
    public class AnalyticsEngine
    {
        //have a list of all the metrics that we are interested in
        private List<IMetric> _metricCollection = null;
        private List<Func<IMetric>> _factorMethods; 
        DataEngine dataEngine;

        public AnalyticsEngine()
        {
            _metricCollection = new List<IMetric>();
            _factorMethods = new List<Func<IMetric>>();
            dataEngine = new DataEngine();
            //dataEngine.SetDataEngine(@"C:\Users\hawk\Desktop\Ruslans Data_1");
        }

        public string LogDirectory { get; set; }

        public string LogFormat { get; set; }

        /// <summary>
        /// Add functions to this list
        /// </summary>
        /// <param name="functor"></param>
        public void AddMetricFactor(Func<IMetric> functor)
        {
            _factorMethods.Add(functor);
        }

        public MetricResult RunAlternativeEngine(DateTime start, DateTime end, TimeSpan timeInterval)
        {
            dataEngine.SetDataEngine(@"C:\Users\t-hawkf\Desktop\Logs\W3SVC1");
            MetricResult metricResults = new MetricResult();
            MakeNewMetricCollection();
            //Thought we are enumerating the data from the parser, the code is less complex if we have it all into a list and then perform our computations. Simple and quicker than the RunEngine method
                //now that we have all of our data in memory go ahead and perform computations on our data with our metrics


            int count = 0;
            while (start < end)
            {
                //if we are going by the hour then add 1 Hour to starttime, by the day then add 1 day to startime, by weekly then add 7 days to starttime, by monthly
                //and by yearly....
                DateTime intermediateTime = start + timeInterval;

                //perform metric computation on all data from [startTime, intermediateTime)
                //afterwards clear the data that are in the metrics and compute metrics for the next set of data
                foreach (W3C_Extended_Log log in dataEngine.GetLines(start, intermediateTime))
                {
                    HelperFunction(start, intermediateTime, log);
                }

                //after HelperFunction is called, the metrics for the data of timestamps [startTime, intermediateTime] should be completed, now organize the data
                foreach (IMetric job in _metricCollection)
                {
                    //make a new object of list for that key
                    if (job != null)
                    {
                        if (!metricResults.MetricNames.Contains(job.MetricName))
                        {
                            metricResults.MetricNames.Add(job.MetricName);
                            metricResults.Result.Add(new List<object>());
                        }
                        if (!metricResults.Times.Contains(start.ToString()))
                        {
                            metricResults.Times.Add(start.ToString());
                        }

                        metricResults.Result[count].Add(job.GetResult());
                        //count keep tracks of which List object we add the result to. Each Metric has its own List object
                        count++;
                    }

                }

                start = intermediateTime;
                MakeNewMetricCollection();
                count = 0;

            }
        return metricResults;
        }

        /// <summary>
        /// Used this function to perform metric computations between a start time and intermediate time
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end">The intermediate time</param>
        /// <param name="log">HttpLog that these metrics will use to compute data</param>
        private void HelperFunction(DateTime start, DateTime end, W3C_Extended_Log log)
        {
            if (log.UTCLogDateTime >= start && log.UTCLogDateTime < end)
            {
                foreach (IMetric job in _metricCollection)
                {
                    job.ProcessEntry(log);
                }
            }
        }

        private void MakeNewMetricCollection()
        {
            _metricCollection.Clear();
            //iterate through the list of functions to create new instances of all the metrics we need
            foreach (Func<IMetric> func in _factorMethods)
            {
                _metricCollection.Add(func());
            }
        }

        /// <summary>
        /// Return the metrics and their descriptions for each and the parameters they need
        /// </summary>
        /// <returns></returns>
        public List<MetricInfo> GetMetricsDescriptions()
        {

            //Dictionary<string,string> imetricTypes = new Dictionary<string,string>();
            List<MetricInfo> imetricTypes = new List<MetricInfo>();

            //use the assembly object to get to the types that inherits IMETRIC
            Assembly webAnalytics = Assembly.Load(typeof(IMetric).Assembly.FullName); //<== successful

            //find the types that are instanceof IMetric
            IMetric metric;
            MetricInfo info;
            foreach (TypeInfo typeInfo in webAnalytics.DefinedTypes)
            {
                if (typeof(IMetric).IsAssignableFrom(typeInfo) && !typeInfo.IsInterface)
                {
                    info = new MetricInfo();
                    //to get a description of the metric, we need to use activator to create an instance of that type and use the GetMetricDescription
                    try
                    {
                        metric = (IMetric)Activator.CreateInstance(typeInfo);
                        info.Name = metric.MetricName;
                        info.Description = metric.GetMetricDescription;
                        info.ClassName = typeInfo.FullName.Replace("WebAnalytics.Model.Metrics.", String.Empty);
                        Trace.WriteLine(info.ClassName);
                        info.Parameters = metric.GetParameters();
                        imetricTypes.Add(info);
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine("error with: " + typeInfo.ToString());
                    }
                }
            }
            return imetricTypes;
        }
    }

    //The MetricInfo and MetricResult class are public so that the internal serializer in the web api may serialize these property names and values successfully

    /// <summary>
    /// Use this class store information for each metric. The metricInfo class must have get accessors so that the internal serializer will use the data. 
    /// </summary>
    public class MetricInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClassName { get; set; }
        public List<MetricParameter> Parameters { get; set; }
    }

    /// <summary>
    /// For chart apis they prefer a mapping of data from multiple arrays. for example.. [Metric1, M2, M3, ...], [M1.Result, M2.Result, M3.Result, ...], [Time1 - Time2, T2-T3, T3-T4, ...]
    /// </summary>
    public class MetricResult
    {
        public List<string> MetricNames = new List<string>();
        public List<string> Times = new List<string>();
        //public Dictionary<string, List<object>> Result = new Dictionary<string, List<object>>();
        public List<List<object>> Result = new List<List<object>>();
        public List<string> Domain = new List<string>();
        public List<object> Range = new List<object>();
        public string TypeOfGraph;
    }

}


