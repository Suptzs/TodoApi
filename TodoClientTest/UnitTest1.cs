using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoClient;

namespace TodoClientTest
{
    [TestClass]
    public class UnitTest1
    {
        private readonly TodoHttpClient _client = new TodoHttpClient();

        private long _todoId;

        [TestMethod]
        public async Task TestFullWebApi()
        {
            await CreateNewTodo();
            await GetAllItems();
            await GetItemById();
            await UpdateItem();
            await DeleteTodo();
        }

        public async Task CreateNewTodo()
        {
            var todo = new TodoItem{Name = "Netflix", IsComplete = false};
            var url = await _client.Add(todo);

            Assert.IsTrue(url.ToString().EndsWith(todo.Key.ToString()));
            Assert.AreEqual("Netflix", todo.Name);
            Assert.IsFalse(todo.IsComplete);

            _todoId = todo.Key;
        }

        public async Task GetAllItems()
        {
            var todos = (await _client.GetAll()).ToList();
            Assert.IsTrue(todos.Count >= 1);

            var todoItem = todos.Single(t => t.Key == _todoId);
            Assert.AreEqual("Netflix", todoItem.Name);
            Assert.IsFalse(todoItem.IsComplete);
        }

        public async Task GetItemById()
        {
            var todoItem = await _client.Find(_todoId);
            Assert.AreEqual("Netflix", todoItem.Name);
            Assert.IsFalse(todoItem.IsComplete);
        }

        public async Task UpdateItem()
        {
            var todo = new TodoItem{Key = _todoId, Name = "Chill", IsComplete = true};
            Assert.IsTrue(await _client.Update(_todoId, todo));

            todo = await _client.Find(_todoId);
            Assert.AreEqual("Chill", todo.Name);
            Assert.IsTrue(todo.IsComplete);
        }

        public async Task DeleteTodo()
        {
            Assert.IsTrue(await _client.Remove(_todoId));
            Assert.IsFalse(await _client.Remove(_todoId));
        }
    }
}
