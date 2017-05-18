using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoClient
{
    public interface IWebApiClient<TEntity, TKey>
    {
        Task<Uri> Add(TEntity todo);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> Find(TKey id);
        Task<bool> Remove(TKey id);
        Task<bool> Update(TKey id, TEntity todo);
    }

    public class WebApiHttpClient<TEntity, TKey> : IWebApiClient<TEntity, TKey>
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _todoUrl;

        public WebApiHttpClient(string baseUrl)
        {
            _todoUrl = baseUrl;
        }

        private string TodoUrl() => _todoUrl;
        private string TodoUrl(TKey id) => $"{_todoUrl}/{id}";

        public async Task<IEnumerable<TEntity>> GetAll()
            => Deserialize<List<TEntity>>(await _client.GetStringAsync(TodoUrl()));

        public async Task<TEntity> Find(TKey id)
            => Deserialize<TEntity>(await _client.GetStringAsync(TodoUrl(id)));

        public async Task<bool> Remove(TKey id)
            => (await _client.DeleteAsync(TodoUrl(id))).IsSuccessStatusCode;

        public async Task<bool> Update(TKey id, TEntity todo)
            => (await _client.PutAsync(TodoUrl(id), CreateJsonContent(todo))).IsSuccessStatusCode;

        /// <exception cref="HttpRequestException">If the IsSuccessStatusCode property for the HTTP response is false</exception>
        public async Task<Uri> Add(TEntity todo)
        {
            var response = await _client.PostAsync(TodoUrl(), CreateJsonContent(todo));
            response.EnsureSuccessStatusCode();

            todo = Deserialize<TEntity>(await response.Content.ReadAsStringAsync());

            return response.Headers.Location;
        }

        protected static ByteArrayContent CreateJsonContent(TEntity todo)
        {
            var byteContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(todo)));
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return byteContent;
        }

        protected static T Deserialize<T>(string serialized) => JsonConvert.DeserializeObject<T>(serialized);
    }
}