using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebAnalytics
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(name: "Find-All-Metrics", routeTemplate: "diagnostics/analytics/metrics", defaults: new { controller = "Analytics", action = "GetAvailableMetrics" });
            config.Routes.MapHttpRoute(name: "General-Analytics", routeTemplate: "diagnostics/analytics/{metrics}/{start}/{end}/{interval}/{*arguments}", defaults: new
            {
                controller = "Analytics",
                action = "GetAnalytics",
                metrics = "",
                start = DateTime.Today.ToUniversalTime(),
                end = DateTime.Now.ToUniversalTime(),
                interval = "1:00",
                arguments = RouteParameter.Optional
            });
        }
    }
}
