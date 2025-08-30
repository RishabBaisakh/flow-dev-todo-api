namespace TodoApi.Models
{
  public class MongoDBSettings
{
    public string ConnectionString { get; set; } = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? string.Empty;
    public string DatabaseName { get; set; } = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME") ?? string.Empty;
}
}