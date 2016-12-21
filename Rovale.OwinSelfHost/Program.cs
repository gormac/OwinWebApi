using System;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.Owin.Hosting;
using Rovale.OwinWebApi;

namespace Rovale.OwinSelfHost
{
    internal class Program
    {
        private static void Main()
        {
            const string baseAddress = "http://localhost:9123/";

            using (WebApp.Start<Startup>(baseAddress))
            {
                // On startup this app will make a request to the self-hosted
                // Web API service. You should see logging statements and results
                // dumped to the console window.
                var client = new HttpClient();
                var response = client.GetAsync(baseAddress + "api/values").Result;

                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
        }
    }
}