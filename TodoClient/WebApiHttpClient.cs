using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoClient
{
    public class WebApiHttpClient<TEntity, TKey> : IWebApiClient<TEntity, TKey> where TEntity : IEntityWithKey<TKey>
    {
        protected readonly HttpClient Client = new HttpClient();
        protected readonly string BaseUrl;

        public WebApiHttpClient(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        protected string Url() => BaseUrl;
        protected string Url(TKey id) => $"{BaseUrl}/{id}";

        public async Task<IEnumerable<TEntity>> GetAll()
            => Deserialize<List<TEntity>>(await Client.GetStringAsync(Url()));

        public async Task<TEntity> Find(TKey id)
            => Deserialize<TEntity>(await Client.GetStringAsync(Url(id)));

        public async Task<bool> Remove(TKey id)
            => (await Client.DeleteAsync(Url(id))).IsSuccessStatusCode;

        public async Task<bool> Update(TKey id, TEntity todo)
            => (await Client.PutAsync(Url(id), CreateJsonContent(todo))).IsSuccessStatusCode;

        /// <exception cref="HttpRequestException">If the IsSuccessStatusCode property for the HTTP response is false</exception>
        public async Task<Uri> Add(TEntity todo)
        {
            var response = await Client.PostAsync(Url(), CreateJsonContent(todo));
            response.EnsureSuccessStatusCode();

            todo.Key = Deserialize<TEntity>(await response.Content.ReadAsStringAsync()).Key;

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