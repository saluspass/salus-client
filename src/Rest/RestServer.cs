/*using System;
using Owin;
using System.Web.Http;
using System.Web.Http.Owin;
using Microsoft.Owin.Hosting;

namespace Salus
{
    public class MyWebSomethingAPIController : ApiController
    {
        [AcceptVerbs("GET")]
        public string Help()
        {
            return "This is test WEB API. Supported methods are ../api/MyWebAPI/Help, ../api/MyWebAPI/Square/{number}";
        }
        [AcceptVerbs("GET")]
        public int Square(int id)
        {
            return id * id;
        }
    }

    class RestServer : ApiController
    {
        #region Constants

        private static readonly Uri _baseAddress = new Uri("http://localhost:60064/");
        private const string baseAddress = "http://localhost:60064/";

        #endregion

        public static void Start()
        {
            WebApp.Start<RestServer>(baseAddress);

            /*ms_Instance = new RestServer();
            ms_Instance.Run();* /
        }

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public new void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }

        [AcceptVerbs("GET")]
        public string Help()
        {
            return "This is test WEB API. Supported methods are ../api/MyWebAPI/Help, ../api/MyWebAPI/Square/{number}";
        }

        /*        public void Configuration(IAppBuilder builder)
                {
                    var config = new HttpConfiguration();
                    config.Routes.MapHttpRoute(
                        "API Default",
                        "api/{controller}/{action}/{id}",
                        new { id = RouteParameter.Optional });
                    builder.UseWebApi(config);
                }* /

        private void Run()
        {

            
            // Set up server configuration
            /*HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(_baseAddress);
            config.Routes.MapHttpRoute(
              name: "DefaultApi",
              routeTemplate: "api/{controller}/{id}",
              defaults: new { id = RouteParameter.Optional }
            );



            // Create server
            var server = new HttpSelfHostServer(config);
            // Start listening
            server.OpenAsync().Wait();
            Console.WriteLine("Web API Self hosted on " + _baseAddress + " Hit ENTER to exit...");
            Console.ReadLine();
            server.CloseAsync().Wait();* /
        }
    }
}*/