import { match } from "ts-pattern";
import { useAppDispatch, useAppSelector } from "./store"
import { useEffect } from "react";
import { Book, bookSlice } from "./slices/books-slice";


const Books = ({ books }: { books: Book[] }) =>
    <> {books.map(e => <span key={e.id}>{JSON.stringify(e)}</span>)} </>


function App() {
    const booksState = useAppSelector(e => e.books);
    const dispatch = useAppDispatch();

    useEffect(() => { dispatch(bookSlice.actions.fetchBooks()) }, []);

    return (
        <div className="container mx-auto pt-10 flex items-center justify-center">
            {match(booksState)
                .with({ state: 'loading' }, _ => <>Loading</>)
                .with({ state: 'error' }, _ => <>Error</>)
                .with({ state: 'success' }, ({ data: books }) => <Books books={books} />)
                // Really helpful technique. 
                // If any other new state is added to books in the feature, 
                // we will have this part of the code failing to compile. 
                // It will force us to reason about what should this page 
                // render once the new state is present.
                .exhaustive()
            }
        </div>
    )
}

export default App
