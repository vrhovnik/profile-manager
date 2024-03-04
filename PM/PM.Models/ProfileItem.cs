namespace PM.Models;

public class ProfileItem
{
    public int ProfileItemId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProfileItemType ItemType { get; set; }
    public string Line { get; set; }
    public string LineContent { get; set; }
    public List<Profile> Profiles { get; set; } = new();
}
