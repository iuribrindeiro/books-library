namespace BookLibrary.Domain.Commands;

//The use of unions will provide Match methods
//to exhaustive match against all possible types.
//Really useful for reacting to domain events.
[Union]
public partial record DomainEvent
{
    public partial record BookInUse(Book Book);
}

public static class UseBookCommand
{
    public static ErrorOr<(Book newBook, DomainEvent[] events)> UseBook(Book book)
        => book.AvailableCopies > 0
            ? AddCopyInUse(book)
            : Error.Conflict("No available copies of the book");

    private static (Book, DomainEvent.BookInUse[]) AddCopyInUse(Book book)
    {
        var newBook = Book.WithCopiesInUse(book, book.CopiesInUse + 1);
        return (newBook, [new DomainEvent.BookInUse(book)]);
    }
}