using BookLibrary.Data;
using BookLibrary.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationContext>(e =>
    e.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => { policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
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

app.MapControllers();

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
        Type = BookType.Hardcover,
        Isbn = "1234567891",
        Category = "Fiction",
        Publisher = "New York Times"
    }
);

context.SaveChanges();

app.Run();