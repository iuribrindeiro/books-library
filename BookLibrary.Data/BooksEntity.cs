using System.ComponentModel.DataAnnotations.Schema;
using BookLibrary.Domain;

namespace BookLibrary.Data;

//Ideally, we would just use the DTOs from the domain to map data to the database.
//I believe that the data in the databse should have no rule at all.
//All rules should be handled by the Core/Domain module. Making it easy to swap from storage.
//And when I say storage, I don't mean just SQL to NoSQL, but also from SQL to a redis cache.
//Anything that stores the data should be abstract to the actual domain logic.
public class BooksEntity
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required AuthorsEntity[] Authors { get; set; }
    public required string Publisher { get; set; }
    public required BookType Type { get; set; }
    public required string Category { get; set; }
    public required string Isbn { get; set; }
    public required int TotalCopies { get; set; }
    public required int CopiesInUse { get; set; }
}