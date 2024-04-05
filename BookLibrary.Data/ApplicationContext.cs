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

            //Using owned entity types makes it easier to map complex types to the database.
            //Again, databases should just store the data, no need to validate foreign keys and such.
            //If we need some performance improvements, we can think of indexes and such.
            .OwnsMany(e => e.Authors, e => e.ToJson());
    }
}
