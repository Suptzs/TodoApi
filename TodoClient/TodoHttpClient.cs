namespace TodoClient
{
    public class TodoHttpClient : WebApiHttpClient<TodoItem, long>
    {
        public TodoHttpClient() : base("http://localhost:52275/api/Todo")
        {
        }
    }
}