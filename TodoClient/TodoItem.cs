namespace TodoClient
{
    public class TodoItem : IEntityWithKey<long>
    {
        public long Key { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}