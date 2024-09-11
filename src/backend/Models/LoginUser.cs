using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace CalisthenicsApi.Models;
public class LoginUser{
    public string Password{get;set;} = null!;
    public string Email{get;set;} = null!;
}