# DCCS.Data.Source &middot; ![Build Status](https://img.shields.io/appveyor/ci/stephanmeissner/dccs-data-source.svg) ![NuGet Version](https://img.shields.io/nuget/v/DCCS.Data.Source.svg)

DCCS.Data.Source helps with automatic sorting and paging of data. It was created, to support Html-DataGrids with sorting and paging.

DCCS.Data.Source is supposed to get out of the programmers way and do as much as possible automaticly.

## Installation

You should install [DCCS.Data.Source with NuGet](https://www.nuget.org/packages/DCCS.Data.Source/):

    Install-Package DCCS.Data.Source

Or via the .NET Core command line interface:

    dotnet add package DCCS.Data.Source

Either commands, from Package Manager Console or .NET Core CLI, will download and install DCCS.Data.Source and all required dependencies.

## Examples

In this example we create an WebAPI action, that takes parameter (`Params`) for paging and sorting information, and returns the sorted and paged data (`Result<T>`).

```csharp
public class UsersController : Controller
{
    public Result<User> Get(Params ps)
    {
        // ...get data i.e. from EF
        // data: IQueryable<User>
        return new Result<User>(ps, data);
    }
}
```

You can also use the IQueryable extension method.

```csharp
    using DCCS.Data.Source;
    ...
    // data must be an IQueryable.
    return data.ToResult<User>();
```

The resulting JSON looks like this:

```javascript
{
    "data": [
        {"name": "user 1", ...},
        {"name": "user 2", ...},
        // ...
    ],
    "page": 1,
    "count": 10,
    "orderBy": "name",
    "desc": false
}
```

You can skip sorting and paging if necessary:
```csharp
return new Result<T>(ps, data, sort: false, page: false);
```

If you need to transform (`Select`) the sorted and paged data, you can use the `Result<T>.Select` method. Like this:

```csharp
...

using(var db = new DbContext()) {
    var users = db.Users;
    return new Result(ps, users)
        .Select(user => new UserDTO(user));
}
```

**Important:** Only the paged data is transformed, so you can do this in combination with EF and it will work performantly with as many rows as your database can handle.

## Contributing

### License

DCCS.Data.Source is [MIT licensed](https://github.com/facebook/react/blob/master/LICENSE)
