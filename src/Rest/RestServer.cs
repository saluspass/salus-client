using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Collections.Generic;
using System.Net.Http.Formatting;
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
            var defaultSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter{ CamelCaseText = true },
                }
            };

            JsonConvert.DefaultSettings = () => { return defaultSettings; };

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings = defaultSettings;

            config.Routes.MapHttpRoute(
                "API Default",
                "{controller}/{action}/{id}",
                new { id = RouteParameter.Optional });
            config.MapHttpAttributeRoutes();

            builder.UseWebApi(config);
        }
    }
}
