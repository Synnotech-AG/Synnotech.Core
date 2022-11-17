# Synnotech.Core
*Provides general abstractions, algorithms, and data structures for .NET*

[![Synnotech Logo](synnotech-large-logo.png)](https://www.synnotech.de/)

[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](https://github.com/Synnotech-AG/Synnotech.DatabaseAbstractions/blob/main/LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-0.3.0-blue.svg?style=for-the-badge)](https://www.nuget.org/packages/Synnotech.DatabaseAbstractions/)

# How to Install

Synnotech.Core is compiled against [.NET Standard 2.0 and 2.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) and thus supports all major plattforms like .NET 5, .NET Core, .NET Framework 4.6.1 or newer, Mono, Xamarin, UWP, or Unity.

Synnotech.Core is available as a [NuGet package](https://www.nuget.org/packages/Synnotech.Core/) and can be installed via:

- **Package Reference in csproj**: `<PackageReference Include="Synnotech.Core" Version="0.3.0" />`
- **dotnet CLI**: `dotnet add package Synnotech.Core`
- **Visual Studio Package Manager Console**: `Install-Package Synnotech.Core`

# What does Synnotech.Core offer you?

## Base classes for Entities

Synnotech.Core offers you four base classes for entities. These are `Int32Entity`, `Int64Entity`, `GuidEntity`, and `StringEntity`. All of them offer an `Id` property of the corresponding type which is immutable by default. Also, all these classes implement `IEntity<T>` (this interface is part of Synnotech.Core) and `IEquatable<T>` for you (two instances are considered equal when they have equal ID values). These base classes are specifically tailored to be used with Object-Relational Mappers or serialization frameworks.

### Deriving from the base classes

A class that derives from these entities could look like this:

```csharp
public sealed class Address : Int32Entity
{
    // Id property is not needed, it comes with the base class
    public string Street { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
}
```

Your class can then be instantiated like so:

```csharp
var address = new Address
{
    Id = 1,
    Street = "Herrmann-Köhl-Str 2a",
    ZipCode = "93049",
    Location = "Regensburg"
};
```

The base classes also offer a parameterized constructor, so you could also make your class immutable via constructor injection (but not many ORMs and serialization frameworks will support that):

```csharp
public sealed class Address : Int32Entity
{
    public Address(int id, string street, string zipCode, string location)
        : base(id)
    {
        Street = street;
        ZipCode = zipCode;
        Location = location;
    }

    public string Street { get; }
    public string ZipCode { get; }
    public string Location { get; }
}
```

### ID range for Int32Entity and Int64Entity

By default, the `Int32Entity` and `Int64Entity` base classes will only allow IDs that are greater or equal to 1.

```csharp
// This statement will throw because Int32Entity and Int64Entity
// only allow positive IDs by default
var address = new Address { Id = 0 };
```

You can customize this behavior by using the static `AllowIdZero` and `AllowNegativeIds` properties:

```csharp
Int32Entity.AllowIdZero = true;
var address = new Address { Id = 0 };
```

There is also a handy `AllowZeroAndNegativeIds` method to set both `AllowIdZero` and `AllowNegativeIds` to true with one call.

> BE CAREFUL: all entities that derive from `Int32Entity` are affected by setting the `AllowIdZero` property or the `AllowNegativeIds` property to `true`. If you want to limit these settings to a specific type, you should derive from `Int32Entity<T>`, like so:

```csharp
public sealed class Address : Int32Entity<Address>
{
    // Members omitted for brevity's sake
}
```

You can then e.g. only allow zero as a valid ID for addresses:

```csharp
// other entities are not affected, because they do not derive from Int32Entity<Address>
Address.AllowIdZero = true; 
var address = new Address { Id = 0 };
var contact = new Contact { Id = 0 }; // This would throw
```

### GuidEntity and empty GUIDs

Similarly to the other types, you can derive from `GuidEntity` or `GuidEntity<T>`:

```csharp
public sealed class Bill : GuidEntity
{
    // Id property is not needed, it comes with the base class
    public decimal AmountInDollar {get; init; }
}
```

By default, `GuidEntity` does not allow empty GUIDs:

```csharp
// The next statement will throw
var bill = new Bill { Id = Guid.EmptyGuid };
```

Similarly to other entity base classes, you can change that by the setting the `AllowEmptyGuid` static property:

```csharp
GuidEntity.AllowEmptyGuids = true;
var bill = new Bill { Id = Guid.EmptyGuid }; // This does not throw
```

As with `AllowIdZero` and `AllowNegativeIds`, the above code would affect all entites deriving from `GuidEntity`. To limit the effect to a single type, you should derive from `GuidEntity<T>`.

### StringEntity, validation, and case-sensitivity

The string entity has the same basic functionality as the other entity base classes. The IDs that are passed to it are validated with the following rules:

- The string must not be null, empty, or contain only white space
- It must be trimmed, i.e. the first and last character must not be white space
- It must have a maximum length of 200 characters

You can customize this behavior by supplying a delegate to the static `ValidateId` property. As always, if you want to limit this to one entity type, consider deriving from `StringEntity<T>` (instead of just `StringEntity`).

Furthermore, by default, an entity operates in case-sensitive mode (to be precise: `StringComparison.Ordinal`). You can change this mode by setting the static `ComparisonMode` property to another value of the `StringComparison` enum. As always: if you want to limit this to certain entity types, consider deriving from `StringEntity<T>`.

> BE CAREFUL: you should only change the comparison mode at the beginning of your application (in the composition root) before any of the entities are instatiated. Otherwise subtle bugs can start to occur (e.g. when the ID is already used as a key in a dictionary), because the `Equals` and `GetHashCode` implementation rely on the `ComparisonMode` value.

The default value for `Id` for a string entity is `null`. You can change this behavior by using the static `IsDefaultValueNull` property whose default value is `true`.

### Changing the ID of an entity after initialization

By default, all ID properties of the entity base classes are immutable. However, there is a back door that you can use to change the ID after the entity is already fully initialized. The usual scenario where this is necessary is when the ID is created by a database so that the ID is only available after an I/O call:

```csharp
await using var session = await SessionFactory.OpenSessionAsync();
var address = new Address
{
    Street = "Herrmann-Köhl-Str 2a",
    ZipCode = "93049",
    Location = "Regensburg"
};
var idOfNewAddress = await session.InsertAsync(address);
await session.SaveChangesAsync();
address.ToMutable().SetId(idOfNewAddress); // This will set the ID after initialization
```

To change the ID after initialization, simply call `entity.ToMutable().SetId(newId)`. `ToMutable` is an extension method which will not polute the public API of your entities.

> BE CAREFUL: you must not change the ID of an entity when it is already supposed to be immutable. This might lead to subtle bugs e.g. when the ID is used as a key in a dictionary.

## Parsing strings to floating point values

.NET already offers many `TryParse` methods when it comes to parsing text to floating point values, but all of them have the issue that they interpret points and commas in a dedicated way (either as decimal sign or as thousand-delimiter sign, depending on the current or provided `CultureInfo`).

But often (and especially in a German context), commas and points might be mixed up, e.g. when users enter text into a text box, or when some IoT devices numbers in the German format, but others in the English format.

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
// This interface is part of Synnotech.Core
public interface IInitializeAsync
{
    bool IsInitialized { get; }

    Task InitializeAsync(CancellationToken cancellationToken = default);
}
```

You can implement this interface in your classes, e.g. like this:

```csharp
public class LinqToDbDatabaseSession : IDatabaseSession, IInitializeAsync
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

You can then register the factory and your type with the DI container:

```csharp
services.AddAsyncFactoryFor<IDatabaseSession, LinqToDbDatabaseSession>();
```

Internally, the type `GenericAsyncFactory<T>` is used to create and initialize your object. This implemmentation also supports scoped lifetimes for your objects.

To use the `IAsyncFactory<T>`, you can simply inject it:

```csharp
public class SomeService
{
    public SomeService(IAsyncFactory<IDatabaseSession> factory) =>
        Factory = factory;

    public IAsyncFactory<IDatabaseSession> Factory { get; }

    public async Task DoSomethingAsync()
    {
        await using var session = await Factory.CreateAsync();
        // Do something useful with your session here
    }
}
```

> Be aware: the implementation above is not thread-safe. You might need to synchronize access within InitializeAsync (e.g. with a `SemaphoreSlim`) if `CreateAsync` is called concurrently from multiple threads and the resulting instance should have a scoped lifetime. We generally recommend to initialize on a single thread and have a dedicated Memory Barrier before `CreateAsync` is called again on another thread (e.g. using an `await` statement).

## Global settings for DI Containers

Synnotech.Core offers a `ContainerSettingsContext` that has a single property called `ContainerSettings`. You can use these settings in your static `AddXXX` (extension) methods that register types with DI containers (usually via the `IServiceCollection` interface). For example, the implementation of `AddAsyncFactoryFor` uses it in the following way:

```csharp
public static IServiceCollection AddAsyncFactoryFor<TAbstraction, TImplementation>(
    this IServiceCollection services,
    ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
    ServiceLifetime factoryLifetime = ServiceLifetime.Singleton,
    bool? registerCreateServiceDelegate = null
)
    where TAbstraction : class
    where TImplementation : TAbstraction
{
    services.MustNotBeNull(nameof(services));

    services.Add(new ServiceDescriptor(typeof(TAbstraction), typeof(TImplementation), serviceLifetime));
    services.Add(new ServiceDescriptor(typeof(IAsyncFactory<TAbstraction>), typeof(GenericAsyncFactory<TAbstraction>),factoryLifetime));

    if (ContainerSettingsContext.Settings.CheckIfFactoryDelegateShouldBeRegistered(registerCreateServiceDelegate))
        services.AddSingleton(container => new Func<TAbstraction>(container.GetRequiredService<TAbstraction>));
    return services;
}
```

In the code example above, the `ContainerSettings` are used to check if a `Func<TAbstraction>` should also be registered with the DI container. Either the caller can specify his intentientions via the optional `registerCreateServiceDelegate` Boolean parameter, or the method will fall back to the default value set on the current instance of `ContainerSettings`. The class also provides a handy `CheckIfFactoryDelegateShouldBeRegistered` method for that.

If you want to change the default container settings, then simply set the property on the ambient context before registering any services to your DI container:

```csharp
ContainerSettingsContext.Settings = new ContainerSettings(false);
// Afterwards, you can call AddXXX on services
```
