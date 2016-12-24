using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
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
        public async void WhenAnItemIsNotFoundItShouldFail()
        {
            using (var server = TestServer.Create<Startup>())
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/api/someObjects/1234");
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public async void WhenAddingAnItemItShouldSucceed()
        {
            using (var server = TestServer.Create<Startup>())
            {
                SomeObject someObject = new SomeObject
                {
                    Id = 1,
                    SomeText = "Some text"
                };

                var content = new ObjectContent<SomeObject>(someObject, new JsonMediaTypeFormatter());
                HttpResponseMessage response = await server.HttpClient.PostAsync("/api/someObjects", content);

                response.StatusCode.Should().Be(HttpStatusCode.Created);
                response.Headers.Location.AbsolutePath.Should().Be("/api/someObjects/1");
            }
        }

        [Fact]
        public async void WhenAddingAnInvalidItemItShouldFail()
        {
            using (var server = TestServer.Create<Startup>())
            {
                SomeObject someObject = new SomeObject
                {
                    Id = 1,
                    SomeText = "123456789012345678901234567890X"
                };

                var content = new ObjectContent<SomeObject>(someObject, new JsonMediaTypeFormatter());
                HttpResponseMessage response = await server.HttpClient.PostAsync("/api/someObjects", content);

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                string result = await response.Content.ReadAsStringAsync();
                result.Should()
                    .Contain("The field SomeText must be a string or array type with a maximum length of \'30\'");
            }
        }

        [Fact]
        public async void WhenAddingADuplicateItemItShouldFail()
        {
            using (var server = TestServer.Create<Startup>())
            {
                SomeObject someObject = new SomeObject
                {
                    Id = 1,
                    SomeText = "Some text"
                };

                await server.HttpClient.PostAsync("/api/someObjects",
                    new ObjectContent<SomeObject>(someObject, new JsonMediaTypeFormatter()));

                HttpResponseMessage response = await server.HttpClient.PostAsync("/api/someObjects",
                    new ObjectContent<SomeObject>(someObject, new JsonMediaTypeFormatter()));

                response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            }
        }
    }
}