using Microsoft.Owin.Hosting;
using Owin;
using System.Web.Http;

namespace Salus
{
    public class RestServer
    {
        /// <summary>
        /// Exposed to starts server using WebApp class which uses OWIN specs.
        /// </summary>
        public void Start()
        {
            WebApp.Start<RestServerImpl>("http://localhost:60064");
        }
    }

    internal class RestServerImpl
    {
        /// <summary>
        /// Method to configure the App to define route and use WebApi
        /// </summary>
        /// <param name="builder"></param>
        /// <remarks>The name of the method should be Configuration and needs to be public.Else there will be exception
        /// A first chance exception of type 'System.EntryPointNotFoundException' occurred in Microsoft.Owin.Hosting.dll
        /// The following errors occurred while attempting to load the app.
        /// - No 'Configuration' method was found in class 'Console45.MyOWINServer, Console45, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'.
        ///</remarks>
        public void Configuration(IAppBuilder builder)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "API Default",
                "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional });
            builder.UseWebApi(config);
        }
    }
}
