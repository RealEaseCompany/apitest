using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace test
{
    public class Address
    {
        public string street { get; set; }
        public string suite { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public Geo geo { get; set; }
    }

    public class Company
    {
        public string name { get; set; }
        public string catchPhrase { get; set; }
        public string bs { get; set; }
    }

    public class Geo
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public Address address { get; set; }
        public string phone { get; set; }
        public string website { get; set; }
        public Company company { get; set; }
    }

    public class Todo
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public bool completed { get; set; }
    }
    public class Program 
    {
        static async Task<List<User>> GetUsers(HttpClient client)
        {
            var users = await client.GetFromJsonAsync<List<User>>("users");
            return users;
        }

        static async Task<List<Todo>> GetTodos(int userid, HttpClient client)
        {
            List<Todo> todos;
            if(userid!=0)
                todos = await client.GetFromJsonAsync<List<Todo>>("todos/?userId=" + Convert.ToString(userid));
            else
                todos = await client.GetFromJsonAsync<List<Todo>>("todos");
            return todos;
        }

        static async Task UpdateTodo(int id,HttpClient client, Todo todo) 
        {
            await client.PutAsJsonAsync("todos/?id=" + Convert.ToString(id), todo);
        } 

        static async Task DeleteTodo(int id, HttpClient client)
        {
            await client.DeleteAsync("todos/?id=" + Convert.ToString(id));
        }

        static async Task PostTodo(HttpClient client, Todo todo)
        {
            await client.PostAsJsonAsync("todos", todo);
        }

        static void Main()
        {
            using HttpClient client = new()
            {
                BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
            };
            
            List<User> users = GetUsers(client).GetAwaiter().GetResult();

            Random rnd = new Random();
            int num = rnd.Next(users.Count);
            Console.WriteLine(num);
            List<Todo> todos = GetTodos(num, client).GetAwaiter().GetResult();
            for (int i = 0; i < todos.Count; i++)
            {
                Console.WriteLine($"Id: {todos[i].id}\nTitle: " +
                $"{todos[i].title}\nCompleted: {todos[i].completed}\n");
            }

            todos = GetTodos(0, client).GetAwaiter().GetResult();
            num = rnd.Next(todos.Count);
            Console.WriteLine(todos[num].id);
            Console.WriteLine(todos[num].completed);
            if (todos[num].completed == true) todos[num].completed = false;
            else todos[num].completed = true;
            UpdateTodo(todos[num].id, client, todos[num]).GetAwaiter().GetResult();
            
            num = rnd.Next(todos.Count);
            Console.WriteLine(todos[num].id);
            DeleteTodo(todos[num].id, client).GetAwaiter().GetResult();
            
            num=rnd.Next(users.Count);
            Todo post = new Todo()
            {
                userId = num,
                id = todos.Count,
                title = "eat",
                completed = true
            };
            PostTodo(client, post).GetAwaiter().GetResult();
        }
    }
}
