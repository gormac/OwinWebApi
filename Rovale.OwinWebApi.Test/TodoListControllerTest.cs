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
    public class TodoListControllerTest
    {
        [Fact]
        public async void WhenGettingJsonItShouldBeCamelCased()
        {
            var todoListProvider = new TodoListProvider();
            todoListProvider.Add(new TodoList { Id = 1, Title = "Some test title 1" });

            using (var server = TestServer.Create(appBuilder =>
            {
                new Startup()
                    .Using(todoListProvider)
                    .Configuration(appBuilder);
            }
            ))
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/api/todoList");
                string result = await response.Content.ReadAsStringAsync();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Should().Be("[{\"id\":1,\"title\":\"Some test title 1\"}]");
            }
        }

        [Fact]
        public async void WhenGettingAllItShouldReturnThem()
        {
            var todoListProvider = new TodoListProvider();
            todoListProvider.Add(new TodoList { Id = 1, Title = "Some test title 1" });
            todoListProvider.Add(new TodoList { Id = 2, Title = "Some test title 2" });
            
            using (var server = TestServer.Create(appBuilder =>
                {
                    new Startup()
                        .Using(todoListProvider)
                        .Configuration(appBuilder);
                }
            ))
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/api/todoList");
                IEnumerable<TodoList> result = (await response.Content.ReadAsAsync<IEnumerable<TodoList>>()).ToArray();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Should().HaveCount(2);
                result.ShouldAllBeEquivalentTo(todoListProvider.GetAll());
            }
        }

        [Fact]
        public async void WhenGettingASingleItemItShouldReturnIt()
        {
            var todoListProvider = new TodoListProvider();
            var todoList1 = new TodoList { Id = 1, Title = "Some test title 1" };
            var todoList2 = new TodoList { Id = 2, Title = "Some test title 2" };

            todoListProvider.Add(todoList1);
            todoListProvider.Add(todoList2);

            using (var server = TestServer.Create(appBuilder =>
            {
                new Startup()
                    .Using(todoListProvider)
                    .Configuration(appBuilder);
            }
            ))
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/api/todoList/2");
                TodoList result = await response.Content.ReadAsAsync<TodoList>();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
                result.ShouldBeEquivalentTo(todoList2);
            }
        }

        [Fact]
        public async void WhenGettingANonExistingItemItShouldFail()
        {
            using (var server = TestServer.Create<Startup>())
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/api/todoList/1234");
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public async void WhenAddingAnItemItShouldSucceed()
        {
            using (var server = TestServer.Create<Startup>())
            {
                TodoList todoList = new TodoList
                {
                    Id = 1,
                    Title = "Some text"
                };

                HttpResponseMessage response = await server.HttpClient.PostAsync("/api/todoList", todoList,
                    new JsonMediaTypeFormatter());

                response.StatusCode.Should().Be(HttpStatusCode.Created);
                response.Headers.Location.AbsolutePath.Should().Be("/api/todoList/1");
            }
        }

        [Fact]
        public async void WhenAddingADuplicateItemItShouldFail()
        {
            using (var server = TestServer.Create<Startup>())
            {
                TodoList todoList = new TodoList
                {
                    Id = 1,
                    Title = "Some text"
                };

                await server.HttpClient.PostAsync("/api/todoList", todoList, new JsonMediaTypeFormatter());

                HttpResponseMessage response = await server.HttpClient.PostAsync("/api/todoList", todoList,
                    new JsonMediaTypeFormatter());

                response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            }
        }

        [Fact]
        public async void WhenDeletingAnExistingItemItShouldSucceed()
        {
            var todoListProvider = new TodoListProvider();
            todoListProvider.Add(new TodoList { Id = 1, Title = "Some test title 1" });

            using (var server = TestServer.Create(appBuilder =>
            {
                new Startup()
                    .Using(todoListProvider)
                    .Configuration(appBuilder);
            }
            ))
            {
                HttpResponseMessage response = await server.HttpClient.DeleteAsync("/api/todoList/1");
                response.StatusCode.Should().Be(HttpStatusCode.NoContent);
                todoListProvider.GetAll().Should().HaveCount(0, "the item should be deleted");
            }
        }

        [Fact]
        public async void WhenDeletingANonExistingItemItShouldFail()
        {
            using (var server = TestServer.Create<Startup>())
            {
                HttpResponseMessage response = await server.HttpClient.DeleteAsync("/api/todoList/1234");
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }
    }
}