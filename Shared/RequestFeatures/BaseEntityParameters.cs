namespace Shared.RequestFeatures;

public class BaseEntityParameters : RequestParameters
{
    public BaseEntityParameters() => OrderBy = "name";
    public uint MinAge { get; set; }
    public uint MaxAge { get; set; } = int.MaxValue;

    public string? SearchTerm { get; set; }

}
