namespace CalisthenicsApi.Models;

public class CalisthenicsDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string UsersCollectionsName { get; set; } = null!;
    public string WorkoutCollectionsName { get; set; } = null!;
}