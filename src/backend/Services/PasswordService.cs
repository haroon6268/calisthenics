namespace CalisthenicsApi.Services;

public class PasswordService(){
    public static string HashPassword(string password) => 
        BCrypt.Net.BCrypt.HashPassword(password);
    
    public static Boolean VerifyPassword(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}