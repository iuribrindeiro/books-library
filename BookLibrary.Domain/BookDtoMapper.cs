using SCurry;

namespace BookLibrary.Domain;

public static class BookDtoMapper
{
    public static BookDto ToBookDto(this Book book) =>
        new()
        {
            Id = book.Id,
            Title = book.Title,
            Authors = book.Authors.Select(ToAuthorDto).ToArray(),
            Publisher = book.Publisher,
            Type = book.Type,
            Category = book.Category,
            Isbn = book.Isbn,
            TotalCopies = book.TotalCopies,
            CopiesInUse = book.CopiesInUse,
            AvailableCopies = book.AvailableCopies
        };

    public static AuthorDto ToAuthorDto(this Author a) =>
        new() { FirstName = a.FirstName, LastName = a.LastName };

    public static ErrorOr<Book> ToBook(this BookDto dto)
        => Id(NewBook).Partial(dto.Id ?? Guid.NewGuid()).Curry()
            .Then(CreateReqStr(dto.Title, nameof(BookDto.Title)))
            .Apply((dto.Authors ?? []).TraverseApply(ToAuthor).ToArray())
            .Apply(CreateReqStr(dto.Publisher, nameof(BookDto.Publisher)))
            .Apply(dto.Type ?? BookType.Hardcover)
            .Apply(CreateReqStr(dto.Category, nameof(BookDto.Category)))
            .Apply(CreateReqStr(dto.Isbn, nameof(BookDto.Isbn)))
            .Apply(dto.TotalCopies ?? 0)
            .Apply(dto.CopiesInUse ?? 0);

    private static ErrorOr<Author> ToAuthor(this AuthorDto dto)
        => Id(NewAuthor).Curry()
            .Then(CreateReqStr(dto.FirstName, nameof(AuthorDto.FirstName)))
            .Apply(CreateReqStr(dto.LastName, nameof(AuthorDto.LastName)));
}
