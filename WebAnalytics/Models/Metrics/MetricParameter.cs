using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAnalytics.Model.Metrics
{
    public struct MetricParameter
    {
        //Interesting. The jsonConvert needs these variables to be public to serialize them successfully, if they are private it cannot access them
        //name of the parameter that our consumers should used to assosciate a value with
        public string Name { get; set; }

        //A description of this parameter so that consumers have better context about what their usings
        public string Description { get; set; }

        //Default value for parameter
        public string Value { get; set; }

        public MetricParameter(string name, string description, string defaultValue):this()
        {
            Name = name;
            Description = description;
            Value = defaultValue;
        }

    }
}
