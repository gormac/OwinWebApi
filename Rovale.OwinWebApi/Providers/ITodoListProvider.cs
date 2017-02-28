using System.Collections.Generic;
using Rovale.OwinWebApi.Models;

namespace Rovale.OwinWebApi.Providers
{
    public interface ITodoListProvider
    {
        IEnumerable<TodoList> GetAll();
        TodoList Find(int id);
        void Add(TodoList todoList);
        void Delete(TodoList todoList);
    }
}