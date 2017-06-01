using Autofac;
using Autofac.Integration.WebApi;
using CognitiveServicesSample.Commons;
using CognitiveServicesSample.Data;
using CognitiveServicesSample.Web.Loggers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CognitiveServicesSample.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            // register types
            builder.RegisterType<CategolizedImageRepository>().As<ICategolizedImageRepository>().InstancePerRequest();
            builder.RegisterType<TraceLogger>().As<ILogger>().SingleInstance();
            builder.Register(c => Options.Create(new CosmosDbSetting
            {
                EndpointUri = ConfigurationManager.AppSettings["CosmosDbSetting.EndpointUri"],
                PrimaryKey = ConfigurationManager.AppSettings["CosmosDbSetting.PrimaryKey"],
            })).SingleInstance();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
