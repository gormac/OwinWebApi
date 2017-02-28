using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Rovale.OwinWebApi.Models;
using Rovale.OwinWebApi.Providers;

namespace Rovale.OwinWebApi.Controllers
{
    [Route("api/todoList")]
    public class TodoListController : ApiController
    {
        private readonly ITodoListProvider _todoListProvider;

        public TodoListController(ITodoListProvider todoListProvider)
        {
            _todoListProvider = todoListProvider;
        }

        public IEnumerable<TodoList> Get()
        {
            return _todoListProvider.GetAll();
        }

        [Route("api/todoList/{id}", Name = "getById")]
        public IHttpActionResult Get(int id)
        {
            var todoList = _todoListProvider.Find(id);

            if (todoList == null)
            {
                return NotFound();
            }

            return Ok(todoList);
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] TodoList todoList)
        {
            if (todoList == null)
            {
                return BadRequest("Invalid passed data");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_todoListProvider.Find(todoList.Id) != null)
            {
                return Conflict();
            }

            _todoListProvider.Add(todoList);

            return CreatedAtRoute("getById", new { id = todoList.Id }, todoList);
        }

        [HttpDelete]
        [Route("api/todoList/{id}")]
        public IHttpActionResult Delete(int id)
        {
            var todoList = _todoListProvider.Find(id);

            if (todoList == null)
            {
                return NotFound();
            }

            _todoListProvider.Delete(todoList);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
