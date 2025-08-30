using TodoApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TodoApi.Services
{
  public class TasksService
  {
    private readonly IMongoCollection<TaskItem> _tasksCollection;

    public TasksService(IOptions<MongoDBSettings> mongoSettings)
    {
      var client = new MongoClient(mongoSettings.Value.ConnectionString);
      var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
      _tasksCollection = database.GetCollection<TaskItem>("Tasks");
    }

    public async Task<List<TaskItem>> GetAsync() =>
      await _tasksCollection.Find(_ => true).ToListAsync();

    public async Task CreateAsync(TaskItem task) =>
      await _tasksCollection.InsertOneAsync(task);

    public async Task<TaskItem?> UpdateStatusAsync(string id, Models.TaskStatus status)
    {
      var updatedTask = await _tasksCollection.FindOneAndUpdateAsync(
        Builders<TaskItem>.Filter.Eq(t => t.Id, id),
        Builders<TaskItem>.Update.Set(t => t.Status, status),
        new FindOneAndUpdateOptions<TaskItem>
        {
          ReturnDocument = ReturnDocument.After
        }
      );

      return updatedTask;
    }
  }
}
