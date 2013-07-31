using System.Collections.Generic;
using WebAnalytics.Model.Logs;

namespace WebAnalytics.Model.Metrics
{
    /// <summary>
    /// IMetric interface provides the underlying structure that programmers and designers need to define in their custom metric
    /// </summary>
    public interface IMetric
    {
        /// <summary>
        /// Every class that implements metric has a name so that when returned we can have an idea of what data is from who
        /// </summary>
        string MetricName { get; set; }
        
        /// <summary>
        /// Every class that implements imetric has a desription about what the metric is about
        /// </summary>
        string GetMetricDescription { get; }

        /// <summary>
        /// Parameters are being sent from the REST api, strip the arguments of the values that this metric may need
        /// </summary>
        /// <param name="args">String arguments of the parameters that this metric may need</param>
        void SetParameters(Dictionary<string,string> args);

        /// <summary>
        /// Every metric will have some level of documentation describing what parameters it takes and a description of the parameter.
        /// The input comes as strings. 
        /// </summary>
        /// <returns>Returns a list of parameters that the metric needs</returns>
        List<MetricParameter> GetParameters();

        /// <summary>
        /// Depending on the metric and how a class derives this method, perform the computations to get the metric information
        /// </summary>
        /// <param name="resource">The Analytics Engine will pass a child class of HttpLogEntry, using this class, strip the fields that are need</param>
        void ProcessEntry(HttpLogEntry entry);

        /// <summary>
        /// After calculations are done, use this method to return the result. API controller automatically serializes the result as JSON
        /// </summary>
        /// <returns></returns>
        object GetResult();
    }
}
