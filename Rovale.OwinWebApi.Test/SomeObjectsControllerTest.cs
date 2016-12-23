using System.Net;
using System.Net.Http;
using FakeItEasy;
using Microsoft.Owin.Testing;
using Rovale.OwinWebApi.Models;
using Rovale.OwinWebApi.Providers;
using Xunit;

namespace Rovale.OwinWebApi.Test
{
    public class SomeObjectsControllerTest
    {
        [Fact]
        public async void WhenGettingAllItShouldReturnThem()
        {
            var someObjectsProvider = A.Fake<ISomeObjectsProvider>();
            A.CallTo(() => someObjectsProvider.GetAll()).Returns(new[]
            {
                new SomeObject {SomeText = "Some test text 1"},
                new SomeObject {SomeText = "Some test text 2"}
            });
             
            using (var server = TestServer.Create(appBuilder =>
                {
                    new Startup()
                        .Using(someObjectsProvider)
                        .Configuration(appBuilder);
                }
            ))
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/api/someObjects");
                string result = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("[{\"someText\":\"Some test text 1\"},{\"someText\":\"Some test text 2\"}]", result);
            }
        }
    }
}