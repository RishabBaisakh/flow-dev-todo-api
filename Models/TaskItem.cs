using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoApi.Models
{
  public class TaskItem
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [BsonRepresentation(BsonType.String)]
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;

    [Required]
    public string AssignedUser { get; set; } = string.Empty;
    // Change the above to a user type so it can be used to switch screens for demo purpose
  }
}