# Synnotech.Core
*Provides general abstractions, algorithms, and data structures for .NET*

[![Synnotech Logo](synnotech-large-logo.png)](https://www.synnotech.de/)

[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](https://github.com/Synnotech-AG/Synnotech.DatabaseAbstractions/blob/main/LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-0.1.0-blue.svg?style=for-the-badge)](https://www.nuget.org/packages/Synnotech.DatabaseAbstractions/)

# How to Install

Synnotech.Core is compiled against [.NET Standard 2.0 and 2.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) and thus supports all major plattforms like .NET 5, .NET Core, .NET Framework 4.6.1 or newer, Mono, Xamarin, UWP, or Unity.

Synnotech.Core is available as a [NuGet package](https://www.nuget.org/packages/Synnotech.Core/) and can be installed via:

- **Package Reference in csproj**: `<PackageReference Include="Synnotech.Core" Version="0.1.0" />`
- **dotnet CLI**: `dotnet add package Synnotech.Core`
- **Visual Studio Package Manager Console**: `Install-Package Synnotech.Core`

# What does Synnotech.Core offer you?

## Parsing strings to floating point values

.NET already offers many `TryParse` methods when it comes to parsing text to floating point values, but all of them have the issue that they interpret points and commas in a dedicated way (either as decimal sign or as thousand-delimiter sign, depending on the current or provided `CultureInfo`).

But often (and especially in a German context), commas and points might be mixed up, e.g. when users enter text into a text box, or when some IoT devices send numbers as text in the German, but others send them in the English format.

You can use the `DoubleParser.TryParse` method which analyses the input string for points and commas and then chooses either the invariant culture or the German culture to parse the string, depending on the number of points and commas and where they are placed. Check out the following code:

```csharp
DoubleParser.TryParse("15.0", out var value);        // value = 15.0
DoubleParser.TryParse("15,0", out var value);        // value = 15.0
DoubleParser.TryParse("200,575.833", out var value); // value = 200575.833
DoubleParser.TryParse("200.575,833", out var value); // value = 200575.833
```

> BE CAREFUL: if you have a number with only a single thousand-delimiter sign (i.e. no decimal sign), this number will not be parsed correctly. The thousand-delimiter sign will be interpreted as the decimal sign. We recognize that this scenario is rare, as especially human input will most likely never use the thousand-delimiter sign. Howevery, if this scenario applies, then please use the .NET `TryParse` methods and specify the correct culture info by yourself.

Synnotech.Core also offers you the `FloatParser` and the `DecimalParser`. Furthermore, the .NET Standard 2.1 version of this library has support for `ReadOnlySpan<char>`.

## IInitializeAsync

If you have objects that need to be initialized asynchronously (these are usually humble objects that asynchronously open a connection to a third-party system, e.g. a database), you can incorporate the `IInitializeAsync` interface in your code:

```csharp
public interface IInitializeAsync
{
    bool IsInitialized { get; }

    Task InitializeAsync(CancellationToken cancellationToken = default);
}
```

You can implement this interface in your classes, e.g. like this:

```csharp
public class LinqToDbDatabaseSession : IInitializeAsync
{
    public LinqToDbDatabaseSession(DataConnection dataConnection) =>
        DataConnection = dataConnection;
    
    public DataConnection DataConnection { get; }

    public bool IsInitialized { get; private set; }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized)
            return;

        await DataConnection.BeginTransactionAsync();
        IsInitialized = true;
    }

    // Further members for database access are omitted for brevity's sake
}
```