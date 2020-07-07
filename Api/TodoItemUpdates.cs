using TodoList.DataAccess;

namespace TodoList.Api
{
    public class TodoItemUpdates
    {
        public int? Id { get; set; }

        public string Name { get; set; }
        
        public bool? IsComplete { get; set; }

        public void ApplyTo(TodoItem todoItem)
        {
            if (Id != null)
            {
                todoItem.Id = Id.Value;
            }

            if (Name != null)
            {
                todoItem.Name = Name;
            }

            if (IsComplete != null)
            {
                todoItem.IsComplete = IsComplete.Value;
            }
        }
    }
}
