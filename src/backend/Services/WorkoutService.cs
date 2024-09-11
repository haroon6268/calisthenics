using CalisthenicsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CalisthenicsApi.Services;

public class WorkoutService
{
    private readonly IMongoCollection<Workout> _workoutCollection;
    public WorkoutService(
        //Settings are grabbed for the IOptions interface, and a connection is made. UserService is registered as a singleton meaning that mongoclient is created once, and connection is maintained
        IOptions<CalisthenicsDatabaseSettings> calisthenicsDatabaseSettings)
    {
        var mongoClient = new MongoClient(calisthenicsDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(calisthenicsDatabaseSettings.Value.DatabaseName);
        _workoutCollection = mongoDatabase.GetCollection<Workout>(calisthenicsDatabaseSettings.Value.WorkoutCollectionsName);
    }
        public async Task<List<Workout>> GetAsync() => 
        await _workoutCollection.Find(x => true).ToListAsync();
    public async Task<Workout> GetByIdAsync(string id) =>
        await _workoutCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task<string> CreateAsync(Workout workout){
        await _workoutCollection.InsertOneAsync(workout);

        //You can access ID because it set before doc is inserted
        return workout.Id;
    }
    public async Task UpdateAsync(string id, Workout workout) => 
        await _workoutCollection.ReplaceOneAsync(x => x.Id == id, workout);
    public async Task RemoveAsync(string id) =>
    await _workoutCollection.DeleteOneAsync(x => x.Id == id);
}