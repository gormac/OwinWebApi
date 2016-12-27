using System.Collections.Generic;
using System.Linq;
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
        public async void WhenGettingJsonItShouldBeCamelCased()
        {
            var someObjectsProvider = new SomeObjectsProvider();
            someObjectsProvider.Add(new SomeObject { Id = 1, SomeText = "Some test text 1" });

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
                result.Should().Be("[{\"id\":1,\"someText\":\"Some test text 1\"}]");
            }
        }

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
                IEnumerable<SomeObject> result = (await response.Content.ReadAsAsync<IEnumerable<SomeObject>>()).ToArray();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Should().HaveCount(2);
                result.ShouldAllBeEquivalentTo(someObjectsProvider.GetAll());
            }
        }

        [Fact]
        public async void WhenGettingASingleItemItShouldReturnIt()
        {
            var someObjectsProvider = new SomeObjectsProvider();
            var someObject1 = new SomeObject { Id = 1, SomeText = "Some test text 1" };
            var someObject2 = new SomeObject { Id = 2, SomeText = "Some test text 2" };

            someObjectsProvider.Add(someObject1);
            someObjectsProvider.Add(someObject2);

            using (var server = TestServer.Create(appBuilder =>
            {
                new Startup()
                    .Using(someObjectsProvider)
                    .Configuration(appBuilder);
            }
            ))
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/api/someObjects/2");
                SomeObject result = await response.Content.ReadAsAsync<SomeObject>();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result.ShouldBeEquivalentTo(someObject2);
            }
        }

        [Fact]
        public async void WhenGettingANonExistingItemItShouldFail()
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

                HttpResponseMessage response = await server.HttpClient.PostAsync("/api/someObjects", someObject,
                    new JsonMediaTypeFormatter());

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

                HttpResponseMessage response = await server.HttpClient.PostAsync("/api/someObjects", someObject,
                    new JsonMediaTypeFormatter());

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

                await server.HttpClient.PostAsync("/api/someObjects", someObject, new JsonMediaTypeFormatter());

                HttpResponseMessage response = await server.HttpClient.PostAsync("/api/someObjects", someObject,
                    new JsonMediaTypeFormatter());

                response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            }
        }

        [Fact]
        public async void WhenDeletingAnExistingItemItShouldSucceed()
        {
            var someObjectsProvider = new SomeObjectsProvider();
            someObjectsProvider.Add(new SomeObject { Id = 1, SomeText = "Some test text 1" });

            using (var server = TestServer.Create(appBuilder =>
            {
                new Startup()
                    .Using(someObjectsProvider)
                    .Configuration(appBuilder);
            }
            ))
            {
                HttpResponseMessage response = await server.HttpClient.DeleteAsync("/api/someObjects/1");
                response.StatusCode.Should().Be(HttpStatusCode.NoContent);
                someObjectsProvider.GetAll().Should().HaveCount(0, "the item should be deleted");
            }
        }

        [Fact]
        public async void WhenDeletingANonExistingItemItShouldFail()
        {
            using (var server = TestServer.Create<Startup>())
            {
                HttpResponseMessage response = await server.HttpClient.DeleteAsync("/api/someObjects/1234");
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }
    }
}