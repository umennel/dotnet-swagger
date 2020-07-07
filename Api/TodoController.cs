using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.DataAccess;


namespace TodoList.Api
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all ToDo Items.
        /// </summary>
        /// <returns>The list of ToDo Item</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        /// <summary>
        /// Gets a specific ToDo Item.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The Item to get</returns>
        /// <response code="404">If the item does not exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            var item = await _context.TodoItems.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        /// <summary>
        /// Creates a ToDo Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Walk the dog",
        ///        "isComplete": false
        ///     }
        ///
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>A newly created ToDo Item</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="409">If the item already exists</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TodoItem>> CreateTodoItem(TodoItem item)
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

        /// <summary>
        /// Updates a ToDo Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/Todo/1
        ///     {
        ///        "id": 1,
        ///        "name": "Walk the dog",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns>No content</returns>
        /// <response code="204">If the update was successful</response>
        /// <response code="400">If the ID does not match</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReplaceTodoItem(int id, TodoItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Partially updates a ToDo Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH api/v1/Todo/1
        ///     {
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="updates"></param>
        /// <returns>No content</returns>
        /// <response code="204">If the update was successful</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTodoItem(int id, TodoItemUpdates updates)
        {
            var item = await _context.TodoItems.FindAsync(id);

            if (item == null) 
            {
                return NotFound();
            }

            updates.ApplyTo(item);

            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific ToDo Item.
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var item = await _context.TodoItems.FindAsync(id);

            if (item != null)
            {
                _context.TodoItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
