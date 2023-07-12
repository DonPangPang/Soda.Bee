namespace Soda.Bee.Shared;

public class QueryParam
{
    public string? SearchKey { get; set; }

    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 20;
}