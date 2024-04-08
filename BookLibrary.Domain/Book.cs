namespace BookLibrary.Domain;

public readonly record struct Book
{
    public required Guid Id { get; init; }
    public required ReqString Title { get; init; }
    public required Author[] Authors { get; init; }
    public required ReqString Publisher { get; init; }
    public required BookType Type { get; init; }
    public required ReqString Category { get; init; }
    public required ReqString Isbn { get; init; }

    //I would use value objects for ints and enums as well, but I just don't have time.
    //ints can be negative (or greater than some values, if we also want to restrict).
    //Enums can have different values than the actual allowed ones, for example,
    //we can implicitly cast an int 3 to a BookType here, the compiler would not complain and we wouldn't get an exception in runtime.
    //Make this validation might be prudent since we don't trust where this data is coming from.
    public required int TotalCopies { get; init; }
    public required int CopiesInUse { get; init; }

    public int AvailableCopies => TotalCopies - CopiesInUse;
    
    public static Book WithCopiesInUse(Book book, int copiesInUse) 
        => book with { CopiesInUse = copiesInUse };

    public static Book NewBook(
        Guid id,
        ReqString title,
        Author[] authors,
        ReqString publisher,
        BookType bookType,
        ReqString category,
        ReqString isbn,
        int totalCopies,
        int copiesInUse
    ) =>
        new()
        {
            Id = id,
            Title = title,
            Authors = authors,
            Publisher = publisher,
            Type = bookType,
            Category = category,
            Isbn = isbn,
            TotalCopies = totalCopies,
            CopiesInUse = copiesInUse
        };
}

public enum BookType
{
    Hardcover,
    Paperback
}

public readonly record struct Author
{
    public required ReqString FirstName { get; init; }
    public required ReqString LastName { get; init; }

    public static Author NewAuthor(ReqString firstName, ReqString lastName) =>
        new() { FirstName = firstName, LastName = lastName };
}
