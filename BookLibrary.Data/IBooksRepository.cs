using BookLibrary.Domain;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Data;

public interface IBooksRepository
{
    public Task<ErrorOr<Book[]>> GetBooksAsync();
}

public class BooksRepository(ApplicationContext appContext) : IBooksRepository
{
    //The use of Task here is really importante, it makes sure that the method is async(not the use of async/await. async only allows the use of await, which is made to await for async code to run before executing the following code).
    //Gathering results from the database could fail, we could consider wrapping this with a Result type as well.
    public async Task<ErrorOr<Book[]>> GetBooksAsync()
    {
        var booksDtos = await appContext.Books
            .Select(e => new BookDto()
            {
                Id = e.Id,
                Authors = e
                    .Authors
                    .Select(a => new AuthorDto() { FirstName = a.FirstName, LastName = a.LastName })
                    .ToArray(),
                Category = e.Category,
                CopiesInUse = e.CopiesInUse,
                Isbn = e.Isbn,
                Publisher = e.Publisher,
                Title = e.Title,
                TotalCopies = e.TotalCopies,
                Type = e.Type,
                AvailableCopies = null //No need to map this here, it is a domain calculated property. Only fill it when Domain -> DTO. Even that we give it a value here, when converting to Domain it will be ignored.
            }).ToArrayAsync();

        return booksDtos
            .Select(BookDtoMapper.ToBook)
            .Transpose(); //Transpose means: ErrorOr<T>[] -> ErrorOr<T[]>
    }
}
