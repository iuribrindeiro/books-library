import { ActionCreatorWithPayload, combineSlices, configureStore } from "@reduxjs/toolkit";
import { Epic, combineEpics, createEpicMiddleware } from "redux-observable";
import { filter } from "rxjs";
import { bookSlice, booksEpics } from "./slices/books-slice";
import { useDispatch, useSelector } from "react-redux";
import { BooksApi, booksClient } from './books-client';

//A little bit of TS magic to get the action types from a slice
type SliceActions<T> = {
    [K in keyof T]: T[K] extends (...args: any[]) => infer A ? A : never;
}[keyof T]

type ActionTypes = SliceActions<typeof bookSlice.actions>

export type AppDispatch = typeof store.dispatch;

type Dependencies = {
    books: BooksApi
}

//filter is a weird name to our context. 
//What we wanna do is get the specified action once it is dispatched.
//onAction sounds more like we are doing something once an action is dispatched.
export const onAction = <T>(action: ActionCreatorWithPayload<T>) => filter(action.match);

const reducer = combineSlices(bookSlice);

export type RootState = ReturnType<typeof reducer>;
//We are basically typing the epic functions so that we never try to use something that doesn't exist because of weak types
export type AppEpic = Epic<ActionTypes, ActionTypes, RootState, Dependencies>;

const epicMiddleware = createEpicMiddleware<ActionTypes, ActionTypes, RootState, Dependencies>({
    dependencies: {
        books: booksClient
    }
});

const rootEpic = combineEpics(...booksEpics);

export const store = configureStore({
    reducer,
    middleware: getDefaultMiddleware => getDefaultMiddleware().concat(epicMiddleware)
})

epicMiddleware.run(rootEpic);

export const useAppDispatch: () => AppDispatch = useDispatch;
export const useAppSelector = useSelector.withTypes<RootState>();
