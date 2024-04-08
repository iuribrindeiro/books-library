using BookLibrary.Domain.Commands;

namespace BookLibrary.Data;

public interface IUnitOfWork
{
    IBooksRepository Books { get; }
    public Task SaveChangesAsync(DomainEvent[] domainEvent);
}

public class UnitOfWork(ApplicationContext appContext) : IUnitOfWork
{
    public IBooksRepository Books { get; } = new BooksRepository(appContext);

    public async Task SaveChangesAsync(DomainEvent[] domainEvents)
    {
        await appContext.SaveChangesAsync();
        await Task.WhenAll(domainEvents.Select(PublishAsync));
    }
    
    private static Task PublishAsync(DomainEvent domainEvent)
    {
        //Publish the event to the message bus/websocket/anything interested.
        return Task.CompletedTask;
    }
}