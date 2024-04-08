using BookLibrary.Data;
using BookLibrary.Domain;
using BookLibrary.Domain.Commands;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Web;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet("/")]
    public Task<IActionResult> GetBooksAsync(SearchCriteriaInputModel inputModel)
        => unitOfWork.Books
            .GetBooksAsync(inputModel)
            .Then(BookDtoMapper.ToBookDtos)
            .ToApiResult(Ok, _ => Problem());
    
    [HttpPost("/use/{id:guid}")]
    public Task<IActionResult> UseBookAsync(Guid bookId)
        => unitOfWork.Books
            .FindBookAsync(bookId)
            .Then(UseBookCommand.UseBook)
            .Tee(e => unitOfWork.Books.UpdateBookAsync(e.newBook))
            .Tee(e => unitOfWork.SaveChangesAsync(e.events))
            .Then(e => e.newBook.ToBookDto())
            .ToApiResult(e => Ok(e), _ => Problem());
}

public readonly record struct SearchCriteriaInputModel
{
    [FromQuery]
    public string? Value { get; init; }
    [FromQuery]
    public SearchCriteriaType? Type { get; init; }
    
    public SearchCriteria ToSearchCriteria() 
        => new() { Value = Value, Type = Type ?? SearchCriteriaType.All };
    
    public static implicit operator SearchCriteria(SearchCriteriaInputModel inputModel) 
        => inputModel.ToSearchCriteria();
}

public static class ApiResultExtensions
{
    public static Task<IActionResult> ToApiResult<T>(
        this Task<ErrorOr<T>> result,
        Func<T, IActionResult> mapOk,
        Func<List<Error>, IActionResult> mapErr
    ) => result.MatchAsync(
        e => Task.FromResult(mapOk(e)), 
        e => Task.FromResult(mapErr(e))
    );
}