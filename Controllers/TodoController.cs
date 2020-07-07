using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TodoList;
using TodoList.DataAccess;


namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                // Create a new TodoItem if collection is empty,
                // which means you can't delete all TodoItems.
                _context.TodoItems.Add(new TodoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem item)
        {
            try {
                _context.TodoItems.Add(item);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentException)
            {
                return Conflict();
            }

            return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);
        }

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, TodoItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Todo/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, Dictionary<string, object> updates)
        {
            var normalizedUpdates = new Dictionary<string, JsonElement?>(updates.Select(kvp => KeyValuePair.Create(kvp.Key.ToLower(), (JsonElement?)kvp.Value)));
            if (normalizedUpdates.ContainsKey("id"))
            {
                return BadRequest();
            }

            var item = await _context.TodoItems.FindAsync(id);

            foreach (var (key, value) in normalizedUpdates)
            {
                if (!value.HasValue) {
                    return BadRequest();
                }

                switch(key) {
                    case "name":
                        if (value?.ValueKind == JsonValueKind.String)
                        {
                            item.Name = value.Value.ToString();
                            break;
                        }
                        
                        return BadRequest();
                    case "iscomplete":
                        if (value.HasValue && value.Value.TryGetBoolean(out var b)) {
                            item.IsComplete = b;
                            break;
                        }
                        
                        return BadRequest();
                    default:
                        return BadRequest();
                }
            }

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NoContent();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
