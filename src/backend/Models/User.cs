using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace CalisthenicsApi.Models;
public class User{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id{ get; set; } 
    public string FirstName{get;set;} = null!;
    public string LastName{get;set;} = null!;
    public string UserName{get;set;} = null!;
    public string Password{get;set;} = null!;
    public string Email{get;set;} = null!;

    public List<string> Workouts{get;set;} = new List<string>();
}