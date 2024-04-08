using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Domain;

public static class FunctionUtils
{
    public static T Id<T>(T val) => val;
    
    public static bool IsEmpty([NotNullWhen(false)]this string? str) 
        => string.IsNullOrWhiteSpace(str);
}