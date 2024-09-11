using CalisthenicsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CalisthenicsApi.Services;

public class UserService
{
    private readonly IMongoCollection<User> _usersCollection;
    public UserService(
        //Settings are grabbed for the IOptions interface, and a connection is made. UserService is registered as a singleton meaning that mongoclient is created once, and connection is maintained
        IOptions<CalisthenicsDatabaseSettings> calisthenicsDatabaseSettings)
    {
        var mongoClient = new MongoClient(calisthenicsDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(calisthenicsDatabaseSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<User>(calisthenicsDatabaseSettings.Value.UsersCollectionsName);
    }
    public async Task<List<User>> GetAsync() => 
        await _usersCollection.Find(x => true).ToListAsync();
    public async Task<User> GetByIdAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task<User> GetByEmailAsync(string email) =>
        await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
    
    public async Task<string> CreateAsync(User user){
        await _usersCollection.InsertOneAsync(user);

        //You can access ID because it set before doc is inserted
        return user.Id;
    }
    public async Task UpdateAsync(string id, User user) => 
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, user);
    public async Task RemoveAsync(string id) =>
    await _usersCollection.DeleteOneAsync(x => x.Id == id);
    
}