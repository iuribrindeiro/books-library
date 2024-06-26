import { match } from "ts-pattern";
import { useAppDispatch, useAppSelector } from "./store"
import { useEffect } from "react";
import { bookSlice } from "./slices/books-slice";
import { Book, SearchCriteria } from "./books-client";


const Books = ({ books }: { books: Book[] }) =>
    <> {books.map(e => <span key={e.id}>{JSON.stringify(e)}</span>)} </>



const SelectSearchCriteria = () => {
    const booksState = useAppSelector(e => e.books);
    const searchCriteria = useAppSelector(bookSlice.selectors.selectSearchCriteria);
    const dispatch = useAppDispatch();

    const setSearchCriteria = (value: SearchCriteria) =>
        dispatch(bookSlice.actions.setSearchCriteria(value));

    return (
        <select
            disabled={booksState.state === 'loading'}
            value={searchCriteria}
            onChange={e => setSearchCriteria(e.target.value as unknown as SearchCriteria)}>
            <option value={SearchCriteria.Title}>Title</option>
            <option value={SearchCriteria.Author}>Author</option>
            <option value={SearchCriteria.Publisher}>Publisher</option>
            <option value={SearchCriteria.ISBN}>ISBN</option>
            <option value={SearchCriteria.All}>All</option>
        </select>
    );
}

function App() {
    const booksState = useAppSelector(e => e.books);
    const dispatch = useAppDispatch();

    useEffect(() => { dispatch(bookSlice.actions.fetchBooks()) }, []);

    return (
        <div className="container mx-auto pt-10 flex items-center justify-center">
            <div>
                <SelectSearchCriteria />
            </div>
            {match(booksState)
                .with({ state: 'loading' }, _ => <>Loading</>)
                .with({ state: 'error' }, _ => <>Error</>)
                .with({ state: 'success' }, ({ data: books }) => <Books books={books} />)
                // Really helpful technique. 
                // If any other new state is added to books in the future, 
                // we will have this part of the code failing to compile. 
                // It will force us to reason about what should this page 
                // render once a new state is present.
                .exhaustive()
            }
        </div>
    )
}

export default App
