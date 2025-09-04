using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using TodoApi.Models;

namespace TodoApi.Services
{
	public class UserService
	{
		private readonly IMongoCollection<User> _usersCollection;

		public UserService(IOptions<MongoDBSettings> mongoSettings)
		{
			var client = new MongoClient(mongoSettings.Value.ConnectionString);
			var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
			_usersCollection = database.GetCollection<User>("Users");
		}

		public async Task<List<User>> GetAsync() =>
			await _usersCollection.Find(_ => true).ToListAsync();

		public async Task<User?> GetByIdAsync(string id)
		{
			if (!ObjectId.TryParse(id, out var objectId))
				return null;

			return await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
		}

		public async Task<List<User>> GetByIdsAsync(List<string> ids)
		{
			var filter = Builders<User>.Filter.In(u => u.Id, ids);
			return await _usersCollection.Find(filter).ToListAsync();
		}

		public async Task CreateAsync(User user) =>
			await _usersCollection.InsertOneAsync(user);

		public async Task UpdateAsync(string id, User updatedUser)
		{
			var filter = Builders<User>.Filter.Eq(u => u.Id, id);
			var update = Builders<User>.Update
				.Set(u => u.Name, updatedUser.Name)
				.Set(u => u.Avatar, updatedUser.Avatar);

			await _usersCollection.UpdateOneAsync(filter, update);
		}

		public async Task<bool> DeleteAsync(string id)
		{
			var result = await _usersCollection.DeleteOneAsync(u => u.Id == id);
			return result.DeletedCount > 0;
		}

		public async Task<bool> CreateIfNotExistsAsync(User user)
		{
			var existingUser = await _usersCollection
				.Find(u => u.Name == user.Name)
				.FirstOrDefaultAsync();

			if (existingUser != null)
			{
				return false; 
			}

			await _usersCollection.InsertOneAsync(user);
			return true;
		}
	}
}
