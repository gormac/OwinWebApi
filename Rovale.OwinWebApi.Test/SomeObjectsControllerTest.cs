using System.Net;
using System.Net.Http;
using FluentAssertions;
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
            var someObjectsProvider = new SomeObjectsProvider();
            someObjectsProvider.Add(new SomeObject { Id = 1, SomeText = "Some test text 1" });
            someObjectsProvider.Add(new SomeObject { Id = 2, SomeText = "Some test text 2" });
            
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

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Should().Be("[" +
                             "{\"id\":1,\"someText\":\"Some test text 1\"}," +
                             "{\"id\":2,\"someText\":\"Some test text 2\"}" +
                             "]");
            }
        }

        [Fact]
        public async void WhenGettingASingleItemItShouldReturnIt()
        {
            var someObjectsProvider = new SomeObjectsProvider();
            someObjectsProvider.Add(new SomeObject { Id = 1, SomeText = "Some test text 1" });
            someObjectsProvider.Add(new SomeObject { Id = 2, SomeText = "Some test text 2" });

            using (var server = TestServer.Create(appBuilder =>
            {
                new Startup()
                    .Using(someObjectsProvider)
                    .Configuration(appBuilder);
            }
            ))
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/api/someObjects/2");
                string result = await response.Content.ReadAsStringAsync();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Should().Be("{\"id\":2,\"someText\":\"Some test text 2\"}");
            }
        }

        [Fact]
        public async void WhenAnItemIsNotFoundItShouldReturnStatusCode404()
        {
            using (var server = TestServer.Create<Startup>())
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/api/someObjects/1234");
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }
    }
}