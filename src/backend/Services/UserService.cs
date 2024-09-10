using CalisthenicsApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CalisthenicsApi.Services;

public class UserService
{
    private readonly IMongoCollection<User> _usersCollection;
    public UserService(
        IOptions<CalisthenicsDatabaseSettings> calisthenicsDatabaseSettings)
    {
        var mongoClient = new MongoClient(calisthenicsDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(calisthenicsDatabaseSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<User>(calisthenicsDatabaseSettings.Value.UsersCollectionsName);
    }
    public async Task<List<User>> GetAsync() => 
        await _usersCollection.Find(x => true).ToListAsync();
    public async Task<User> GetAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task CreateAsync(User user) =>
        await _usersCollection.InsertOneAsync(user);
    public async Task UpdateAsync(string id, User user) => 
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, user);
    public async Task RemoveAsync(string id) =>
    await _usersCollection.DeleteOneAsync(x => x.Id == id);
    
}