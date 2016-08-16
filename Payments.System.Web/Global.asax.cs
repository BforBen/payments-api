using System;
using System.Web.Routing;

namespace Payments.System.Web
{
    public class Global : global::System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.LowercaseUrls = true;

            //var Logger = new LoggerConfiguration()
            //    .WriteTo.File(HttpContext.Current.Server.MapPath("~/App_Data/log.txt"))
            //    .Enrich.WithMachineName()
            //    .Enrich.With<HttpRequestUrlEnricher>()
            //    .Enrich.WithThreadId()
            //    .Enrich.With<HttpRequestClientHostIPEnricher>()
            //    .Enrich.WithProcessId()
            //    .Enrich.With<HttpRequestIdEnricher>()
            //    .Enrich.With<HttpRequestTypeEnricher>()
            //    .Enrich.With<HttpRequestUserAgentEnricher>()
            //    .Enrich.With<HttpRequestUrlReferrerEnricher>();

            //Log.Logger = Logger.CreateLogger();

            //Log.Debug("Payments API started.");
        }
    }
}