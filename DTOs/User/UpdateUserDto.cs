using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs.User
{
  public class UpdateUserDto
  {
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Avatar { get; set; } = string.Empty;
  }
}
