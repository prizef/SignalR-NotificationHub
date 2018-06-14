using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

[assembly: OwinStartupAttribute(typeof(Web.Startup))]
namespace Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            
            // Use Web API dependency resolver to create an instance of SignalR hub
            GlobalHost.DependencyResolver.Register(
                typeof(NotificationHub),
                () => GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(NotificationHub))
            );

            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);
        }
    }
}
