using System.ComponentModel.DataAnnotations;
using TodoApi.Validation;

namespace TodoApi.DTOs
{
  public class CreateTaskDto
  {
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [EnumValue(typeof(Models.TaskStatus))]
    public string Status { get; set; } = string.Empty;

    [Required]
    public string AssignedUser { get; set; } = string.Empty;
  }
}