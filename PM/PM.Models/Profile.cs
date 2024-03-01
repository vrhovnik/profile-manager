namespace PM.Models;

public class Profile
{
    public int ProfileId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProfileType Type { get; set; }
}