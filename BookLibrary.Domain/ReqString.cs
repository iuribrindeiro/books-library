//Using a value object to hold valid data is much better than using the builtin C# string type.
//This type here ensures me that if an instance exists in runtime,
//there is no way that it can be an empty string instance.
//
//Also, in case we want to retrieve the ReqString from a string? or string type, we MUST match the result of ErrorOr<ReqString>
//If the string is empty, we get an error instead.
//Safe to use valid values by the C# compiler.

namespace BookLibrary.Domain;

public record ReqString
{
    public static ErrorOr<ReqString> CreateReqStr(string? value, string fieldName) =>
        string.IsNullOrWhiteSpace(value)
            ? Error.Validation($"{fieldName} cannot be empty")
            : new ReqString(value);

    private readonly string value;

    private ReqString(string val) => value = val;

    public override string ToString() => value;

    public static implicit operator string(ReqString rs) => rs.value;
}
