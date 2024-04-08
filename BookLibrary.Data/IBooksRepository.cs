using System.Linq.Expressions;
using BookLibrary.Domain;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Data;

public interface IBooksRepository
{
    public Task<ErrorOr<Book[]>> GetBooksAsync(SearchCriteria searchCriteria);
    public Task UpdateBookAsync(Book book);
    public Task<ErrorOr<Book>> FindBookAsync(Guid bookId);
}

public readonly record struct SearchCriteria
{
    public required string? Value { get; init; }
    public required SearchCriteriaType Type { get; init; }
}

public enum SearchCriteriaType
{
    Title,
    Author,
    Publisher,
    Isbn,
    All
}

public class BooksRepository(ApplicationContext appContext) : IBooksRepository
{
    //The use of Task here is really importante, it makes sure that the method is async(not the use of async/await. async only allows the use of await, which is made to await for async code to run before executing the following code).
    //Gathering results from the database could fail, we could consider wrapping this with a Result type as well.
    public async Task<ErrorOr<Book[]>> GetBooksAsync(SearchCriteria searchCriteria)
    {
        var booksDtos = await 
            ApplySearchCriteria(appContext.Books, searchCriteria)
            //AsNoTracking improves performance because it won't be using the change tracker,
            //so entites loaded here won't be tracked in memory.
            //It should be used only in queries where we know for sure that won't suffer any changes.
            //This is the most efficiente feature from EF Core, it should only be disabled with careful.
            .AsNoTracking()    
            .Select(e => ToBookDto(e))
            .ToArrayAsync();

        return booksDtos
            .Select(BookDtoMapper.ToBook)
            .Transpose(); //Transpose means: ErrorOr<T>[] -> ErrorOr<T[]>
    }

    public async Task UpdateBookAsync(Book book)
    {
        //This is not necessaraly going to the database.
        //The EF DbContext implements a Unit of Work pattern,
        //so it will only go to the database when the entity is not already being tracked.
        var entityFromDb = await appContext.Books.FindAsync(book.Id);
        
        if (entityFromDb is null)
        {
            //We could return a Result type here as well.
            throw new InvalidOperationException("Book not found.");
        }
        
        appContext.Entry(entityFromDb).CurrentValues.SetValues(ToBooksEntity(book));
    }

    public async Task<ErrorOr<Book>> FindBookAsync(Guid bookId)
    {
        var book = await appContext.Books.FindAsync(bookId);
        
        return book is null
            ? Error.NotFound("Book not found.")
            : ToBookDto(book).ToBook();
    }

    private static IQueryable<BooksEntity> ApplySearchCriteria(
        IQueryable<BooksEntity> query, SearchCriteria searchCriteria)
        => searchCriteria switch
        {
            { Value: var val } when val.IsEmpty() => query,
            { Type: SearchCriteriaType.Title, Value: {} val }
                => query.Where(e => e.Title.Contains(val)),
            { Type: SearchCriteriaType.Author, Value: { } val }
                => query.Where(AnyAuthorMatchingCriteria(val)),
            { Type: SearchCriteriaType.Publisher, Value: { } val }
                => query.Where(e => e.Publisher.Contains(val)),
            { Type: SearchCriteriaType.Isbn, Value: { } val }
                => query.Where(e => e.Isbn.Contains(val)),
            _ => query
        };

    private static BookDto ToBookDto(BooksEntity e)
        => new()
        {
            Id = e.Id,
            Authors = e
                .Authors.Select(a => new AuthorDto()
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName
                })
                .ToArray(),
            Category = e.Category,
            CopiesInUse = e.CopiesInUse,
            Isbn = e.Isbn,
            Publisher = e.Publisher,
            Title = e.Title,
            TotalCopies = e.TotalCopies,
            Type = e.Type,
            AvailableCopies = null //No need to map this here, it is a domain calculated property.
            //Only fill it when Domain -> DTO.
            //Even that we give it a value here, when converting to Domain it will be ignored.
        };

    private static BooksEntity ToBooksEntity(Book book)
        => new()
        {
            Authors = book.Authors
                .Select(a => new AuthorsEntity()
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToArray(),
            Category = book.Category,
            CopiesInUse = book.CopiesInUse,
            Id = book.Id,
            Isbn = book.Isbn,
            Publisher = book.Publisher,
            Title = book.Title,
            TotalCopies = book.TotalCopies,
            Type = book.Type
        };

    private static Expression<Func<BooksEntity, bool>> AnyAuthorMatchingCriteria(string val)
        => e
            => e.Authors.Any(a =>
                a.FirstName.Contains(val)
                || a.LastName.Contains(val)
            );
}