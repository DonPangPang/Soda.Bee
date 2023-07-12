namespace Soda.Bee.Shared;

public class User
{
    public string ConnectionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ICollection<string> GroupIds { get; set; } = new List<string>();
}