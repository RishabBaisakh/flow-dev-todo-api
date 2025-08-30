using DotNetEnv;
using System.Text.Json.Serialization;
using TodoApi.Models;
using TodoApi.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// MongoDB config
builder.Services.Configure<MongoDBSettings>(
builder.Configuration.GetSection("MongoDbSettings"));

// Services
builder.Services.AddSingleton<TasksService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt =>
{
  opt.AddPolicy("AllowAll", p =>
  p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers()
  .AddJsonOptions(options =>
  {
      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
  });

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();