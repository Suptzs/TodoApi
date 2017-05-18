using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoClient
{
    public class TodoHttpClient
    {
        private readonly HttpClient _client = new HttpClient();

        private static string TodoUrl() => "http://localhost:52275/api/Todo";
        private static string TodoUrl(long id) => $"{TodoUrl()}/{id}";

        public async Task<List<TodoItem>> GetAllTodos()
            => Deserialize<List<TodoItem>>(await _client.GetStringAsync(TodoUrl()));

        public async Task<TodoItem> GetTodoById(long id)
            => Deserialize<TodoItem>(await _client.GetStringAsync(TodoUrl(id)));

        public async Task<bool> DeleteTodo(long id)
            => (await _client.DeleteAsync(TodoUrl(id))).IsSuccessStatusCode;

        public async Task<bool> UpdateTodo(long id, TodoItem todo)
            => (await _client.PutAsync(TodoUrl(id), CreateJsonContent(todo))).IsSuccessStatusCode;

        /// <summary>Create a TodoItem</summary>
        /// <returns>Url of created TodoItem and the created TodoItem</returns>
        /// <exception cref="HttpRequestException">If the IsSuccessStatusCode property for the HTTP response is false</exception>
        public async Task<Uri> CreateTodoItem(TodoItem todo)
        {
            var response = await _client.PostAsync(TodoUrl(), CreateJsonContent(todo));
            response.EnsureSuccessStatusCode();

            todo.Key = Deserialize<TodoItem>(await response.Content.ReadAsStringAsync()).Key;

            return response.Headers.Location;
        }

        private static ByteArrayContent CreateJsonContent(TodoItem todo)
        {
            var byteContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(todo)));
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return byteContent;
        }

        private static T Deserialize<T>(string serialized) => JsonConvert.DeserializeObject<T>(serialized);
    }
}