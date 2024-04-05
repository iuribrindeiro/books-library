using BookLibrary.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Data;

public class ApplicationContext : DbContext
{
    public DbSet<BooksEntity> Books { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BooksEntity>()
            //Making use of copilot to convert these into .HasData method calls.
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

            //Using owned entity types makes it easier to map complex types to the database.
            //Again, databases should just store the data, no need to validate foreign keys and such.
            //If we need some performance improvements, we can think of indexes and such.
            .OwnsMany(e => e.Authors)
            .HasData(new BooksEntity()
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
                }, new BooksEntity()
                {
                    Id = Guid.NewGuid(),
                    Title = "To Kill a Mockingbird",
                    Authors = [new AuthorsEntity() { FirstName = "Harper", LastName = "Lee" }],
                    TotalCopies = 75,
                    CopiesInUse = 65,
                    Type = BookType.Paperback,
                    Isbn = "1234567892",
                    Category = "Fiction",
                    Publisher = "New York Times"
                }, new BooksEntity()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Catcher in the Rye",
                    Authors = [new AuthorsEntity() { FirstName = "J.D.", LastName = "Salinger" }],
                    TotalCopies = 50,
                    CopiesInUse = 45,
                    Type = BookType.Hardcover,
                    Isbn = "1234567893",
                    Category = "Fiction",
                    Publisher = "New York Times"
                }, new BooksEntity()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Great Gatsby",
                    Authors = [new AuthorsEntity() { FirstName = "F. Scott", LastName = "Fitzgerald" }],
                    TotalCopies = 50,
                    CopiesInUse = 22,
                    Type = BookType.Hardcover,
                    Isbn = "1234567894",
                    Category = "Non-Fiction",
                    Publisher = "New York Times"
                }, new BooksEntity()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Alchemist",
                    Authors = [new AuthorsEntity() { FirstName = "Paulo", LastName = "Coelho" }],
                    TotalCopies = 50,
                    CopiesInUse = 35,
                    Type = BookType.Hardcover,
                    Isbn = "1234567895",
                    Category = "Biography",
                    Publisher = "New York Times"
                }, new BooksEntity()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Book Thief",
                    Authors = [new AuthorsEntity() { FirstName = "Markus", LastName = "Zusak" }],
                    TotalCopies = 75,
                    CopiesInUse = 11,
                    Type = BookType.Hardcover,
                    Isbn = "1234567896",
                    Category = "Mystery",
                    Publisher = "New York Times"
                }, new BooksEntity()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Chronicles of Narnia",
                    Authors = [new AuthorsEntity() { FirstName = "C.S.", LastName = "Lewis" }],
                    TotalCopies = 100,
                    CopiesInUse = 14,
                    Type = BookType.Paperback,
                    Isbn = "1234567897",
                    Category = "Sci-Fi",
                    Publisher = "New York Times"
                },
                new BooksEntity()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Da Vinci Code",
                    Authors = [new AuthorsEntity() { FirstName = "Dan", LastName = "Brown" }],
                    TotalCopies = 50,
                    CopiesInUse = 40,
                    Type = BookType.Paperback,
                    Isbn = "1234567898",
                    Category = "Sci-Fi",
                    Publisher = "New York Times"
                },
                new BooksEntity()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Grapes of Wrath",
                    Authors = [new AuthorsEntity() { FirstName = "John", LastName = "Steinbeck" }],
                    TotalCopies = 50,
                    CopiesInUse = 35,
                    Type = BookType.Hardcover,
                    Isbn = "1234567899",
                    Category = "Fiction",
                    Publisher = "New York Times"
                },
                new BooksEntity()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Hitchhiker's Guide to the Galaxy",
                    Authors = [new AuthorsEntity() { FirstName = "Douglas", LastName = "Adams" }],
                    TotalCopies = 50,
                    CopiesInUse = 35,
                    Type = BookType.Paperback,
                    Isbn = "1234567900",
                    Category = "Non-Fiction",
                    Publisher = "New York Times"
                });
    }
}