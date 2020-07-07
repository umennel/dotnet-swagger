using System.ComponentModel.DataAnnotations;

namespace TodoList.DataAccess
{
    public class TodoItem
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public bool IsComplete { get; set; }
    }
}
