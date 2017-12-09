using System;

namespace Salus
{
    class RestServer
    {
        #region Constants

        private static readonly Uri _baseAddress = new Uri("http://localhost:60064/");

        #endregion

        #region Variables

        private static RestServer ms_Instance;

        #endregion

        public static void Start()
        {
            ms_Instance = new RestServer();
            ms_Instance.Run();
        }

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
            server.CloseAsync().Wait();*/
        }
    }
}
