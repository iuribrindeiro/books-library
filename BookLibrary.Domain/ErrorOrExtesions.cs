using SCurry;

namespace BookLibrary.Domain;

public static class ErrorOrExtesions
{
    public static ErrorOr<TOutput> Apply<TInput, TOutput>(
        this ErrorOr<Func<TInput, TOutput>> resultFunc,
        ErrorOr<TInput> result
    ) =>
        (resultFunc, result) switch
        {
            ({ IsError: false } successLeft, { IsError: false } successRight)
                => successLeft.Value(successRight.Value),
            ({ IsError: true } errorLeft, { IsError: true } errorRight)
                => errorLeft.Errors.Concat(errorRight.Errors).ToArray(),
            ({ IsError: true } error, _) => error.Errors,
            (_, { IsError: true } error) => error.Errors
        };
    
    public static ErrorOr<TOutput> Apply<TInput, TOutput>(
        this ErrorOr<Func<TInput, TOutput>> resultFunc,
        TInput input)
        => resultFunc.Apply(input.ToResult());
    
    public static ErrorOr<IEnumerable<TResult>> TraverseApply<TResult, TValue>(
        this IEnumerable<TValue> items,
        Func<TValue, ErrorOr<TResult>> func
    )
    {
        var cons = (TResult item, IEnumerable<TResult> remaining) => remaining.Prepend(item);
        
        return items.ToArray() switch
        {
            [] => Array.Empty<TResult>(),
            [var first, .. var remaining]
                => Apply(cons.Curry().ToResult(), func(first))
                    .Apply(remaining.TraverseApply(func))
        };
    }
    
    public static ErrorOr<T[]> Transpose<T>(this IEnumerable<ErrorOr<T>> results) 
        => results.TraverseApply(e => e).ToArray();
    
    public static ErrorOr<T[]> ToArray<T>(this ErrorOr<IEnumerable<T>> items) => items.Then(e => e.ToArray());
    
    public static ErrorOr<T> ToResult<T>(this T val) => val;
    
    public static ErrorOr<TResult> Then<T, TResult>(this Func<T, TResult> func, ErrorOr<T> result)
        => result.Then(func);
}