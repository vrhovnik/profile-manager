namespace PM.Models;

public class ProfileType
{
    public string ProfileTypeId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Count { get; set; } = 0;
}