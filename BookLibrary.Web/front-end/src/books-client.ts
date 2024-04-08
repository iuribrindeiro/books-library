import { from } from "rxjs";

export type Book = {
    id: string,
    title: string,
    authors: { firstName: string, lastName: string }[],
    publisher: string,
    type: BookType,
    category: string,
    isbn: string,
    totalCopies: number,
    copiesInUse: number,
    availableCopies: number
}

enum BookType {
    Hardcover,
    Paperback
}

export enum SearchCriteria {
    Title,
    Author,
    Publisher,
    ISBN,
    All
}

export const booksClient = {
    fetchBooks: (searchCriteria: SearchCriteria) =>
        from(
            fetch(`http://localhost:5013/books?searchBy=${searchCriteria}`)
                .then((e) => e.json() as unknown as Book[])
        )
}

export type BooksApi = typeof booksClient;
