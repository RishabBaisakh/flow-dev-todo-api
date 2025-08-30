using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers
{
  [ApiController]
  [Route("tasks")]
  public class TasksController : ControllerBase
  {
    private readonly TasksService _tasksService;

    public TasksController(TasksService tasksService)
    {
      _tasksService = tasksService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskDto dto)
    {
      if (!Enum.TryParse<Models.TaskStatus>(dto.Status, ignoreCase: true, out var status))
      {
          return BadRequest(new { 
              Status = $"'{dto.Status}' is not valid. Allowed values are: ToDo, InProgress, Completed." 
          });
      }
      var task = new TaskItem
      {
        Title = dto.Title,
        Description = dto.Description,
        Status = status,
        AssignedUser = dto.AssignedUser
      };

      await _tasksService.CreateAsync(task);

      return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskItem>>> GetTasks()
    {
      var tasks = await _tasksService.GetAsync();
      return Ok(tasks);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateTaskStatus(string id, [FromBody] UpdateTaskStatusDto dto)
    {
      if (!Enum.TryParse<Models.TaskStatus>(dto.Status, ignoreCase: true, out var status))
      {
          return BadRequest(new { 
              Status = $"'{dto.Status}' is not valid. Allowed values are: ToDo, InProgress, Completed." 
          });
      }

      var updatedTask = await _tasksService.UpdateStatusAsync(id, status);
      if (updatedTask == null) return NotFound();

      return Ok(updatedTask);
    }
  }
}
