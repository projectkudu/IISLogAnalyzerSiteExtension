/// <reference path="jquery-1.10.2.min.js"/>
/// <reference path="jquery.jqplot.min.js"/>

/*
AnalyticsManager follows a sequence of steps to perform an AJAX call to the api and 
1. We got all of the available metrics.
2. We got a start and end datetime.
3. Formed arguments for our AJAX call.
4. Call the API for a result.
5. Refresh the view models metrics array with new data
6. Chart numerical values from the metrics
*/


//Use this function as a constructor for Metric objects. Using the information stored in this object
// the apiClassName will be a direct map to the div that this metric is graphed to
// the name and description will be used to describe the metric in the same area as the graph
// the parameters are used to create fields where users may input data for the next Analytics API call
// the plot is the numerical data for graphing using jqplot
function Metric(name, description, apiClassName, parameters) {
    var plot;
    this.name = name;
    this.description = description;
    this.apiClassName = apiClassName;
    this.parameters = parameters;

}

//create custom binding for jqplot
//TODO make the jqplotchart smart in such a way to graph diverse graphs
ko.bindingHandlers.jqPlotChart = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        //get the current value property assosciated with this DOM object
        var metric = ko.unwrap(valueAccessor());
        var plot = metric.plot;
        $('#' + element.id).html('');
        $.jqplot(element.id, [plot], {
            series: [{
                showMarker: true,
                pointLabels: { show: true }
            }],
            axes: {
                xaxis: {
                    renderer: $.jqplot.DateAxisRenderer,
                    label: metric.name,
                    tickInterval: ViewModel.requestedIntervals.tickInterval
                }
            },
            animate: true
        });
    }
};

//our ViewModel for different units of TIME that a user can use for the Analytics tool
var ViewModel = {
    unitTimes: ko.observableArray(['7 days', '24 Hours', '6 Hours', 'Last Month']),
    //all of the metrics that we will be ploting
    metrics: ko.observableArray(),
    metricPlots: ko.observableArray(),
    uniqueMetricNames: ko.observableArray(),
    selectedTime: ko.observable(),
    getMetricsURL: "diagnostics/analytics/metrics",
    metricInformation: new Array(),

    metricUniqueNames: new Array()
};
ViewModel.requestedDates = ko.computed(function () {
    var end = new Date();
    var start = new Date();
    var dayOfMonth = end.getDate();
    var hours = end.getHours();
    var month = end.getMonth();
    //depending on the unit of time the user selected, calculate the begin date
    switch (this.selectedTime()) {
        case "7 days":
            start.setDate(dayOfMonth - 6);
            break;
        case "24 Hours":
            start.setHours(hours - 23);
            break;
        case "6 Hours":
            start.setHours(hours - 5);
            break;
        case "Last Month":
            start.setMonth(month - 1);
            break;
    }

    var dateSet = new Object();
    dateSet.start = start.toUTCString();
    dateSet.end = end.toUTCString();
    return dateSet;
}, ViewModel);
ViewModel.requestedIntervals = ko.computed(function () {
    var valueModelJson = ko.toJS(ViewModel);
    var timeInterval,tickInterval;
    switch (this.selectedTime()) {
        case "7 days":
            tickInterval = "1 day";
            timeInterval = "1";
            break;
        case "24 Hours" || "6 Hours":
            tickInterval = "1 hour";
            timeInterval = "1:00";
            break;
        case "Last Month":
            tickInterval = "7 day";
            timeInterval = "1";
            break;
    }

    var intervals = new Object();
    intervals.tickInterval = tickInterval;
    intervals.timeInterval = timeInterval;
    return intervals;
}, ViewModel);

var plot
var resources = {
    //api url for calling the api to retrieve all the metrics and their description
    getMetricsURL: "diagnostics/analytics/metrics",
    getParametersURL: "diagnostics/analytics/{metric}/parameters",
    metricInformation: new Array(),
    metricUniqueNames: new Array()
};

//START state diagram sequence
function AnalyticsScriptEngine() {
    //Start by getting the available metrics
    run();
}


//Get all the available metrics that the Analytics API supports.
function run() {
    var arrayMetrics;
    //use jQuery AJAX getJSON call to get the available metrics
    var viewModelJSON = ko.toJS(ViewModel);

    //Get all the supported metrics and their information such as description of metric, parameters, and default value for parameters
    $.getJSON(viewModelJSON.getMetricsURL, function (data) {
        //the data already comes in as JSON. Api controllers in webapi automatically serializes returned data as JSON.
        var count = 0;
        var name, description, parameters, className;
        var metric;
        console.info(data);
        //iterate through each of the metrics
        data.forEach(function (metric) {
            name = metric.Name;
            description = metric.Description;
            parameters = metric.Parameters;
            className = metric.ClassName;
            var temp = new Metric(name, description, className, parameters);
            //see if this metric already exist in the observalble array, if not then add it in. In the next iteration of getting the available metrics, we will push to the array only what has been updated as metrics to the API.
            if (ViewModel.uniqueMetricNames.indexOf(className) == -1) {
                ViewModel.metricInformation.push(temp);
                ViewModel.uniqueMetricNames.push(temp.apiClassName);
            }
        });

        //After the AJAX call to get all the metrics, form parameters to make another AJAX call to get computed data
        var ajaxArguments = formParametersPerMetric();

        //get start and end times and the intervals we want to send to the Analytics API from the ViewModel
        var dates = viewModelJSON.requestedDates;
        var start = dates.start;
        var end = dates.end;
        var interval = viewModelJSON.requestedIntervals;
        var metricList = "";
        var i = 0, size;

        //We added the available metrics to the ViewModel so we need to get an up to date state of the viewModel. Use ko.toJS to do that.
        viewModelJSON = ko.toJS(ViewModel);
        size = viewModelJSON.uniqueMetricNames.length;

        //Part of the API call requires the class names of the metrics defined in code. So create a list of the class names of all the metrics the API supports
        viewModelJSON.uniqueMetricNames.forEach(function (metricName) {
            metricList = metricList + metricName;
            if (i != size - 1) {
                metricList += ',';
                i++;
            }
        });
        console.info(metricList);

        callAnalyticsAPI(metricList, start, end, interval.timeInterval, ajaxArguments, function (data) {
            console.info(data);

            //Given the data, form an array that can be interpreted by jQplot to chart
            formJqPlotData(data);

            //we've already made an AJAX call to send the persisted data from the observables, now clear the array and push the new data in it
            ViewModel.metrics.removeAll();
            ViewModel.metricInformation.forEach(function (metric) {
                ViewModel.metrics.push(metric);
            });

            console.info(ViewModel.metrics.len)
        });
    })
};

//Make an AJAX call to the API and get plot information for each metric
function callAnalyticsAPI(metricList, startTime, endTime, intervalTime, metricArguments, parentCallBack) {
    var apiURL = "diagnostics/analytics";
    var result;
    $.get(apiURL, { metrics: metricList, start: startTime, end: endTime, interval: intervalTime, arguments: metricArguments },
        function callback(data, textStatus) {
            //Send data back to suscribers
            parentCallBack(data);
        })
}

///Form the parameters for calling the API, using JSON syntax
function formParametersPerMetric() {
    var build = "{";

    //iterate over each metric and grab its parameters and cocatenate it to build
    ViewModel.metricInformation.forEach(function (metric) {
        if (metric.parameters != null) {
            metric.parameters.forEach(function (arg) {
                build = build + '"' + arg.Name + '":' + '"' + arg.Value + '"}';
                //create multiple entries for data, comma delimeted
                build = build + ",";
            });
        }
    });

    build = build.slice(0, build.length - 1);
    return build;
}

//Given, an array that has array of integer and double values, form plot data that can be read by jqPlot
//TODO make the formJQplotData smart in such a way to plot diverse graphs
function formJqPlotData(apiResult) {
    var viewModelJSON = ko.toJS(ViewModel);
    var mapNames = [];
    var allMetricPlots = new Array();
    var xVal, yVal;
    var jsonVersion = JSON.parse(apiResult);
    //iterate through the Times and the Result and merge the two to form a plot
    var times = jsonVersion.Times;
    var result = jsonVersion.Result;
    var metrics = jsonVersion.MetricNames;
    var metricPlotValues = new Array();

    if (metrics.length == result.length) {
        var innerResult, x, y;
        for (var i = 0; i < result.length; i++) {
            //get the inner array
            innerResult = result[i];
            var plotData = [];
            //Times and innerResult are parallel to each other
            for (var j = 0; j < times.length; j++) {
                x = times[j];
                y = innerResult[j];
                if (mapNames.indexOf(metrics[i]) == -1) {
                    mapNames.push(metrics[i]);
                }
                plotData.push([x, y]);
            }
            //find the metric that equals this metric name and assign the plotdata to that metric
            ViewModel.metricInformation.forEach(function (metric) {
                if (metric.name == metrics[i]) {
                    metric.plot = plotData;
                }
            });
        }
    }
}



