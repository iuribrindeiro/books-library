import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import { catchError, map, mergeMap, of } from "rxjs";
import { AppEpic, onAction } from "../store";

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

type BookModel =
    | { state: 'loading' }
    | { state: 'success', data: Book[] }
    | { state: 'error' }

const initialState = { state: 'loading' } satisfies BookModel as BookModel;

export const bookSlice = createSlice({
    name: 'books',
    initialState,
    reducers: {
        fetchBooks: (_) => ({ state: 'loading' }),

        setBooks: (_, action: PayloadAction<Book[]>) =>
            ({ state: 'success', data: action.payload }),

        setError: (_) => ({ state: 'error' })
    }
})


//Notice how easy this function is to test.
//It takes 3 arguments, an observable of actions, the current state, as observable, 
//and an object that will hold the actual dependencies to interact with impure data, such as an API call.
//Giving fake values to it is simple and it should alway return the same value if the same set of parameters is given.
export const fetchBooksEpic: AppEpic = (action$, _, deps) =>
    action$.pipe(
        //Once an action of type `fetchBooks` is present
        onAction(bookSlice.actions.fetchBooks),
        //Perform the following behavior:
        mergeMap(_ =>
            //From the books dependency, call fetchBooks
            deps.books.fetchBooks().pipe(
                //Once the result is returned, map it to a setBooks action
                map(bookSlice.actions.setBooks),
                //if the result is actually an error, map it to a setError action
                catchError(_ => of(bookSlice.actions.setError()))
            )
        )
    )
