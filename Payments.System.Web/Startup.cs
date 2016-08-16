﻿using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using Microsoft.Owin.Extensions;

using Serilog;
using SerilogWeb.Classic.Enrichers;

[assembly: OwinStartup(typeof(Payments.System.Web.Startup))]
namespace Payments.System.Web
{
    public class Startup
    {
        ILogger logger;

        public void Configuration(IAppBuilder app)
        {
            #region WebAPI Config

            var apiConfig = new HttpConfiguration();

            // Load controllers from other assemblies
            apiConfig.Services.Replace(typeof(IAssembliesResolver), new DefaultAssembliesResolver());

            // Map attribute routes
            apiConfig.MapHttpAttributeRoutes();
            apiConfig.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // Set JSON
            apiConfig.Formatters.Clear();
            apiConfig.Formatters.Add(new JsonMediaTypeFormatter());
            apiConfig.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            #endregion

            const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} ({RequestId}) {Message}{NewLine}{Exception}";

            logger =
                new LoggerConfiguration().Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.With<HttpRequestUrlEnricher>()
                .Enrich.WithThreadId()
                .Enrich.With<HttpRequestClientHostIPEnricher>()
                .Enrich.WithProcessId()
                .Enrich.With<HttpRequestIdEnricher>()
                .Enrich.With<HttpRequestTypeEnricher>()
                .Enrich.With<HttpRequestUserAgentEnricher>()
                .Enrich.With<HttpRequestUrlReferrerEnricher>()
                    .WriteTo
                        .RollingFile(
                        global::System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/log.txt"),
                            outputTemplate: DefaultOutputTemplate)
                    .WriteTo.Seq("http://localhost:5341", apiKey: "YnyH2ZrSrOl2GaIpyipZ", bufferBaseFilename: global::System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Logs"))
                    .CreateLogger();


            app.UseSerilogRequestContext();
            app.UseSerilog(logger);

            app.UseWebApi(apiConfig);
            app.UseStageMarker(PipelineStage.MapHandler);

            Log.Information("Test");
        }
    }
}
