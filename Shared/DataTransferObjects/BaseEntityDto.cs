namespace Shared.DataTransferObjects;

public record BaseEntityDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? FullAddress { get; init; }
}

public record BaseEntityForCreationDto
{
    public string? Name { get; init; }
    public string? Address { get; init; }
    public string? Country { get; init; }
    public IEnumerable<DependantEntityForCreationDto>? DependantEntities { get; init; }
}

public record BaseEntityForUpdateDto
{
    public string? Name { get; init; }
    public string? Address { get; init; }
    public string? Country { get; init; }
    public IEnumerable<DependantEntityForCreationDto>? DependantEntities { get; init; }
}
