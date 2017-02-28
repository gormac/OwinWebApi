using System.Collections.Generic;
using System.Linq;
using Rovale.OwinWebApi.Models;

namespace Rovale.OwinWebApi.Providers
{
    public class TodoListProvider : ITodoListProvider
    {
        private readonly List<TodoList> _todoLists = new List<TodoList>();

        public IEnumerable<TodoList> GetAll()
        {
            return _todoLists;
        }

        public TodoList Find(int id)
        {
            return _todoLists.SingleOrDefault(o => o.Id == id);
        }

        public void Add(TodoList todoList)
        {
            _todoLists.Add(todoList);
        }

        public void Delete(TodoList todoList)
        {
            _todoLists.Remove(todoList);
        }
    }
}