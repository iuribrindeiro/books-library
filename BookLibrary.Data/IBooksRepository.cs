using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Data;

public interface IBooksRepository
{
    public Task<BooksEntity[]> GetBooksAsync();
}

public class BooksRepository(ApplicationContext appContext) : IBooksRepository
{
    //No need to async/await here.
    //async/await doesn't make the method async, it allows the method to await on internal Tasks (others async methods)
    //to finish and do something else after it. We don't want to do anything else after the ToArrayAsync, so we don't need to await it.
    //Using async/await just for using here would allocate unecessary memory in runtime.
    //The use of Task is really importante tho, it makes sure that the method is async.
    //Gathering results from the database could fail, we could consider wrapping this with a Result type as well.
    public Task<BooksEntity[]> GetBooksAsync() 
        => appContext.Books.ToArrayAsync();
}
