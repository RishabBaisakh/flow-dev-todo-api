using TodoApi.DTOs.User;

namespace TodoApi.DTOs.Task
{
	public class TaskDto
	{
		public string Id { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public string AssignedUserId { get; set; } = string.Empty;
	}
}