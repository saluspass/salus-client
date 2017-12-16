using System;
using System.Net;

namespace CsharpListenerDemo
{
    static class CsharpListenerExample
    {
        static HttpListener web;

        public static void Entry()
        {
            web = new HttpListener();

            web.Prefixes.Add("http://localhost:1271/");
            web.Prefixes.Add("https://localhost:8443/");

            Console.WriteLine("Listening..");

            web.Start();

            Console.WriteLine(web.GetContext());

            var context = web.GetContext();

            var response = context.Response;

            const string responseString = "<html><body>Hello world</body></html>";

            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;

            output.Write(buffer, 0, buffer.Length);

            Console.WriteLine(output);

            output.Close();

            web.Stop();

            Console.ReadKey();
        }
    }
}