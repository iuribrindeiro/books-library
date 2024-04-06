using BookLibrary.Data;
using BookLibrary.Domain;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationContext>(e
    => e.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IBooksRepository, BooksRepository>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.MapGet("/books", (IBooksRepository booksRepository)
        => booksRepository
            .GetBooksAsync() //Get the books from the database as a domain type, so we can make sure not to work with invalid data 
            .Then(MapToBookDtos) //Map the list of Books to Dtos, which is a more "raw" data type that the API can handle. We could also define specific data types for the API layer, so the swagger docs can be more concise to what the API actually returns.
            .ToApiResult(Results.Ok, errs => Results.Problem()) //Conver the final result type to an API result. At the end here, we might have failed to convert the Book from the database, so respond with a Problem. There could be some more fancy error matching here.
).WithOpenApi()
.Produces<BookDto[]>();

static IEnumerable<BookDto> MapToBookDtos(Book[] books)
    => books.Select(BookDtoMapper.ToBookDto);

using var ctx = app.Services.CreateScope();

var context = ctx.ServiceProvider.GetRequiredService<ApplicationContext>();

context.Database.Migrate();

//Clear the database so at every startup it looks new. Probably should only use it in Development
context.Books.Select(e => e).ExecuteDelete();

context.Books.AddRange(
        new BooksEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Pride and Prejudice",
            Authors = [new AuthorsEntity() { FirstName = "Jane", LastName = "Austen" }],
            TotalCopies = 100,
            CopiesInUse = 80,
            Type = BookLibrary.Domain.BookType.Hardcover,
            Isbn = "1234567891",
            Category = "Fiction",
            Publisher = "New York Times"
        });

context.SaveChanges();

app.Run();

public static class ApiResultExtensions
{
    public static Task<IResult> ToApiResult<T>(this Task<ErrorOr<T>> result,
            Func<T, IResult> mapOk, Func<List<Error>, IResult> mapErr)
        => result.MatchAsync(
                e => Task.FromResult(mapOk(e)),
                e => Task.FromResult(mapErr(e))
            );
}
