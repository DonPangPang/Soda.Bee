namespace Soda.Bee.Shared;

public class Group
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = "什么描述也没有";
}
