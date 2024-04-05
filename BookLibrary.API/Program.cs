using BookLibrary.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

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
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();

app.MapGet("/books", (IBooksRepository booksRepository) => booksRepository.GetBooksAsync());


//Making use of copilot to convert these into default data method calls.
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('Pride and Prejudice', 'Jane', 'Austen', 100, 80, 'Hardcover', '1234567891', 'Fiction');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('To Kill a Mockingbird', 'Harper', 'Lee', 75, 65, 'Paperback', '1234567892', 'Fiction');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('The Catcher in the Rye', 'J.D.', 'Salinger', 50, 45, 'Hardcover', '1234567893', 'Fiction');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('The Great Gatsby', 'F. Scott', 'Fitzgerald', 50, 22, 'Hardcover', '1234567894', 'Non-Fiction');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('The Alchemist', 'Paulo', 'Coelho', 50, 35, 'Hardcover', '1234567895', 'Biography');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('The Book Thief', 'Markus', 'Zusak', 75, 11, 'Hardcover', '1234567896', 'Mystery');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('The Chronicles of Narnia', 'C.S.', 'Lewis', 100, 14, 'Paperback', '1234567897', 'Sci-Fi');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('The Da Vinci Code', 'Dan', 'Brown', 50, 40, 'Paperback', '1234567898', 'Sci-Fi');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('The Grapes of Wrath', 'John', 'Steinbeck', 50, 35, 'Hardcover', '1234567899', 'Fiction');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('The Hitchhiker''s Guide to the Galaxy', 'Douglas', 'Adams', 50, 35, 'Paperback', '1234567900', 'Non-Fiction');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('Moby-Dick', 'Herman', 'Melville', 30, 8, 'Hardcover', '8901234567', 'Fiction');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('To Kill a Mockingbird', 'Harper', 'Lee', 20, 0, 'Paperback', '9012345678', 'Non-Fiction');
//
//
// INSERT INTO books (title, first_name, last_name, total_copies, copies_in_use, type, isbn, category)
// VALUES ('The Catcher in the Rye', 'J.D.', 'Salinger', 10, 1, 'Hardcover', '0123456789', 'Non-Fiction');

using var ctx = app.Services.CreateScope();
var context = ctx.ServiceProvider.GetRequiredService<ApplicationContext>();

context.Database.Migrate();

context.Books.Select(e => e).ExecuteDelete();

context.Books.AddRange(
        new BooksEntity() { 
            Id = Guid.NewGuid(), 
            Title = "Pride and Prejudice", 
            Authors = [new AuthorsEntity() { FirstName = "Jane", LastName = "Austen" }],
            TotalCopies = 100, 
            CopiesInUse = 80, 
            Type = BookLibrary.Domain.BookType.Hardcover,
            Isbn =  "1234567891", 
            Category = "Fiction",
            Publisher = "New York Times"
        });

context.SaveChanges();

app.MapFallbackToFile("index.html");

app.Run();
