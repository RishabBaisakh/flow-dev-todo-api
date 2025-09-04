using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.DTOs.Task;
using TodoApi.Services;
using TodoApi.DTOs;

namespace TodoApi.Controllers
{
  [ApiController]
  [Route("tasks")]
  public class TasksController : ControllerBase
  {
    private readonly TasksService _tasksService;
    private readonly UserService _userService;

    public TasksController(TasksService tasksService, UserService userService)
    {
      _tasksService = tasksService;
      _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskDto createTaskDto)
    {
      if (!Enum.TryParse<Models.TaskStatus>(createTaskDto.Status, ignoreCase: true, out var status))
      {
        return BadRequest(new
        {
          Status = $"'{createTaskDto.Status}' is not valid. Allowed values: ToDo, InProgress, Completed."
        });
      }

      var assignedUser = await _userService.GetByIdAsync(createTaskDto.AssignedUserId);
      if (assignedUser == null)
      {
        return BadRequest(new { Error = "Assigned user does not exist." });
      }

      var task = new TaskItem
      {
        Title = createTaskDto.Title,
        Description = createTaskDto.Description,
        Status = status,
        AssignedUserId = createTaskDto.AssignedUserId
      };

      await _tasksService.CreateAsync(task);

      var result = new
      {
        task.Id,
        task.Title,
        task.Description,
        Status = task.Status.ToString(),
        AssignedUser = new
        {
          assignedUser.Id,
          assignedUser.Name,
          assignedUser.Avatar
        }
      };

      return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, result);
    }


    [HttpGet("{id:length(24)}")]
    public async Task<IActionResult> GetTaskById(string id)
    {
      var task = await _tasksService.GetByIdAsync(id);
      if (task == null) return NotFound();

      var user = await _userService.GetByIdAsync(task.AssignedUserId);

      var result = new
      {
        task.Id,
        task.Title,
        task.Description,
        Status = task.Status.ToString(),
        AssignedUser = user != null ? new
        {
          user.Id,
          user.Name,
          user.Avatar
        } : null
      };

      return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
    {
      if (pageNumber < 1) pageNumber = 1;
      if (pageSize < 1) pageSize = 5;

      var tasks = await _tasksService.GetAsync();
      var sortedTasks = tasks.OrderByDescending(t => t.Id).ToList();

      var pagedTasks = sortedTasks
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToList();

      var userIds = pagedTasks.Select(t => t.AssignedUserId).Distinct().ToList();
      var users = await _userService.GetByIdsAsync(userIds);

      var result = pagedTasks.Select(task => new
      {
        task.Id,
        task.Title,
        task.Description,
        Status = task.Status.ToString(),
        AssignedUser = users
          .Where(u => u.Id == task.AssignedUserId)
          .Select(u => new { u.Id, u.Name, u.Avatar })
          .FirstOrDefault()
      });

      var totalTasks = sortedTasks.Count;
      var totalPages = (int)Math.Ceiling(totalTasks / (double)pageSize);

      return Ok(new
      {
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalPages = totalPages,
        TotalTasks = totalTasks,
        Tasks = result
      });
    }

    [HttpPatch("{id:length(24)}/status")]
    public async Task<IActionResult> UpdateTaskStatus(string id, [FromBody] UpdateTaskStatusDto updateStatusDto)
    {
      if (!Enum.TryParse<Models.TaskStatus>(updateStatusDto.Status, ignoreCase: true, out var status))
      {
        return BadRequest(new
        {
            Status = $"'{updateStatusDto.Status}' is not valid. Allowed values: ToDo, InProgress, Completed."
        });
      }

      var updatedTask = await _tasksService.UpdateStatusAsync(id, status);
      if (updatedTask == null) return NotFound();

      return Ok(updatedTask);
    }
  }
}
