using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace CalisthenicsApi.Models;

public class Workout{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? User{get;set;}
    public string DateOfWorkout{get;set;} = null!;
    public List<Exercise> Workouts{get;set;} = new List<Exercise>();

}
