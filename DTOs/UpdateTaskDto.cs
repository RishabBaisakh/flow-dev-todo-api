using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TodoApi.Validation;

namespace TodoApi.DTOs
{
  public class UpdateTaskStatusDto
  {
    [Required]
    [EnumValue(typeof(Models.TaskStatus))]
    public string Status { get; set; } = string.Empty;
  }
}