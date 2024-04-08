import { PayloadAction, createSlice } from "@reduxjs/toolkit";
import { catchError, map, mergeMap, of, take } from "rxjs";
import { AppEpic, onAction } from "../store";
import { Book, SearchCriteria } from "../books-client";


type BookModel =
    | { state: 'loading', searchCriteria: SearchCriteria }
    | { state: 'success', searchCriteria: SearchCriteria, data: Book[] }
    | { state: 'error', searchCriteria: SearchCriteria }

const initialState = {
    state: 'loading',
    searchCriteria: SearchCriteria.All
} satisfies BookModel as BookModel;

export const bookSlice = createSlice({
    name: 'books',
    initialState,
    reducers: {
        fetchBooks: (state) =>
            ({ ...state, state: 'loading' }),

        setBooks: (state, action: PayloadAction<Book[]>) =>
            ({ ...state, state: 'success', data: action.payload }),

        setError: (state) => ({ ...state, state: 'error' }),

        setSearchCriteria: (state, action: PayloadAction<SearchCriteria>) =>
            ({ ...state, searchCriteria: action.payload })
    },
    selectors: {
        selectSearchCriteria: state => state.searchCriteria,
    }
})


//Notice how easy this function is to test.
//It takes 3 arguments, an observable of actions, the current state, as observable, 
//and an object that will hold the actual dependencies to interact with impure data, such as an API call.
//Giving fake values to it is simple and it should alway return the same value if the same set of parameters is given.
const fetchBooksEpic: AppEpic = (action$, state$, deps) =>
    action$.pipe(
        //Once an action of type `fetchBooks` is present
        onActions2(bookSlice.actions.fetchBooks, bookSlice.actions.setSearchCriteria),
        //Perform the following behavior:
        mergeMap(_ =>
            //reading the state changes
            state$.pipe(
                //we get the first occurrence of the state
                take(1),
                //map it to a searchCriteria value
                map(bookSlice.selectors.selectSearchCriteria),
            )
        ),
        //After that, we just get what we got and map it to a call to fetchBooks
        mergeMap(searchCriteria =>
            //From the books dependency, call fetchBooks
            deps.books.fetchBooks(searchCriteria || SearchCriteria.All).pipe(
                //Once the result is returned, map it to a setBooks action
                map(bookSlice.actions.setBooks),
                //if the result is actually an error, map it to a setError action
                catchError(_ => of(bookSlice.actions.setError()))
            )
        )
    )

const fetchBooksOnChangeSearchCriteria: AppEpic = (action$, _, __) =>
    action$.pipe(
        onAction(bookSlice.actions.setSearchCriteria),
        map(({ payload: newSearchCriteria }) => ({ newSearchCriteria })),
        map(_ => bookSlice.actions.fetchBooks())
    )

export const booksEpics = [fetchBooksEpic, fetchBooksOnChangeSearchCriteria];
