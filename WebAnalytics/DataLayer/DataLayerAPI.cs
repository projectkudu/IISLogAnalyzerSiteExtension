using System;
using System.Collections.Generic;
using WebAnalytics.Model.Logs;

namespace WebAnalytics.DataLayer
{
    public interface IDataLayerAPI
    {
        /// <summary>
        /// Using the start time and end time to filter the data that we need for Metric computation
        /// </summary>
        /// <param name="start">A beginning time that the user specifies</param>
        /// <param name="end">An end time to stop retrieving data </param>
        /// <returns>Yield return an instance of IEnumerable to work on the raw data as the foreach loop is iterating</returns>
        IEnumerable<W3C_Extended_Log> GetLines(DateTime start, DateTime end);
    }
}
