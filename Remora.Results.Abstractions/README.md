Remora.Results.Abstractions
==============

Remora.Results.Abstractions contains the common abstract API surfaces for the Remora.Results algebraic data types. See
[Remora.Results](../Remora.Results) for more information.

## Usage
To create your own result type, there are a few considerations to make. See the example below.

```c#
public class MyResult : ResultBase<MyResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyResult"/> class.
    /// </summary>
    private MyResult()
    {
    }

    /// <inheritdoc cref="ResultBase{TResultType}(string,Exception)"/>
    [UsedImplicitly]
    private MyResult
    (
        string? errorReason,
        Exception? exception = null
    )
        : base(errorReason, exception)
    {
    }

    public static DetermineConditionResult FromSuccess()
    {
        return new DetermineConditionResult();
    }
}
```
All result types shall have a constructor taking a nullable string, and a nullable exception. The base class uses this
constructor to instantiate failed results with a message and an optional exception, as well as a mechanism for wrapping
existing failed results of other types.

The typical API for a result type is one or more static overloads of the `FromError` method, which behave appropriately
for their failure states, as well as one or more overloads of the `FromSuccess` method, which takes any potential values
the user wishes to return with a successful operation.

To see examples of more complex types, see [Remora.Results](../Remora.Results).

## Building
The library does not require anything out of the ordinary to compile.

```bash
cd $SOLUTION_DIR
dotnet build
dotnet pack -c Release
```

## Downloading
Get it on [NuGet][1].


[1]: https://www.nuget.org/packages/Remora.Results.Abstractions/
