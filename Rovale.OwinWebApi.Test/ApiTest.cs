using System.Net;
using System.Net.Http;
using Microsoft.Owin.Testing;
using Rovale.OwinWebApi;
using Rovale.OwinWebApi.Test;
using Xunit;

namespace Web.Tests
{
    public class ApiTest
    {
        [Fact]
        public async void Test()
        {
            using (var server = TestServer.Create(appBuilder =>
                {
                    new Startup()
                        .Using(new TestValuesProvider())
                        .Configuration(appBuilder);
                }
            ))
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/api/values");
                string result = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("[\"test value1\",\"test value2\"]", result);
            }
        }
    }
}