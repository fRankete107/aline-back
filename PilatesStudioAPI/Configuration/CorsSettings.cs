namespace PilatesStudioAPI.Configuration;

public class CorsSettings
{
    public List<string> AllowedOrigins { get; set; } = new();
    public bool AllowCredentials { get; set; } = true;
}