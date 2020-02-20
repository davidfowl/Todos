namespace Todos
{
    public class Todo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsComplete { get; set; } = false;
    }
}
