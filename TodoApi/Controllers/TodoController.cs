using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly ITodoRepository _todoRepository;

        public TodoController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        /// <summary> Returns a Collection of TodoItems</summary>
        /// <returns>All TodoItems</returns>
        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return _todoRepository.GetAll();
        }

        /// <summary>Returns a specific TodoItem</summary>
        /// <param name="id"></param>
        /// <returns>The TodoItem with the specified id</returns>
        /// <response code="200">Returns the specified item</response>
        /// <response code="404">The item does not exist</response>
        [HttpGet("{id}", Name = "GetTodo")]
        [ProducesResponseType(typeof(TodoItem), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetById(long id)
        {
            var item = _todoRepository.Find(id);
            if (item == null)
                return NotFound();
            return new ObjectResult(item);
        }

        /// <summary> Creates a TodoItem </summary>
        /// <remarks> Note that the key is optional and will be overwritten by the system.</remarks>
        /// <param name="item"></param>
        /// <returns>New Created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">The item or the name is null</response>
        [HttpPost]
        [ProducesResponseType(typeof(TodoItem), 201)]
        [ProducesResponseType(400)]
        public IActionResult Create([FromBody, Required] TodoItem item)
        {
            if (item?.Name == null)
                return BadRequest();

            item.Key = 0;
            _todoRepository.Add(item);

            return CreatedAtRoute("GetTodo", new {id = item.Key}, item);
        }

        /// <summary>Updates a specific TodoItem</summary>
        /// <remarks>The id must match the items key</remarks>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <response code="204">The Item was successfully updated</response>
        /// <response code="400">The Item is null or its key does not match the id</response>
        /// <response code="404">The Item does not exist</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoItem item)
        {
            if (item == null || item.Key != id)
                return BadRequest();

            var todo = _todoRepository.Find(id);
            if (todo == null)
                return NotFound();

            todo.IsComplete = item.IsComplete;
            todo.Name = item.Name;
            _todoRepository.Update(todo);

            return new NoContentResult();
        }

        /// <summary>Deletes a specific TodoItem</summary>
        /// <param name="id"></param>
        /// <response code="204">The Item was successfully deleted</response>
        /// <response code="404">The Item does not exist</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(long id)
        {
            var todo = _todoRepository.Find(id);
            if (todo == null)
                return NotFound();

            _todoRepository.Remove(id);
            return new NoContentResult();
        }
    }
}
