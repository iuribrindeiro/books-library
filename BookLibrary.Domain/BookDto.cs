namespace BookLibrary.Domain;

//DTOs are data that comes from outside the domain.
//We don't trust this data, it doesn't matter where it came from, DB, API,
//any place that is not the domain is untrusted data.
public readonly record struct BookDto
{
    public required Guid? Id { get; init; }
    public required string? Title { get; init; }
    public required AuthorDto[]? Authors { get; init; }
    public required string? Publisher { get; init; }
    public required BookType? Type { get; init; }
    public required string? Category { get; init; }
    public required string? Isbn { get; init; }
    public required int? TotalCopies { get; init; }
    public required int? CopiesInUse { get; init; }
    public required int? AvailableCopies { get; init; }
}

public readonly record struct AuthorDto
{
    public required string? FirstName { get; init; }
    public required string? LastName { get; init; }
}
