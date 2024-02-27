namespace PM.Models;

public class Profile
{
    public string ProfileId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProfileType Type { get; set; }
}