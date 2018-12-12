# DCCS.Data.Source

![Build Status](https://img.shields.io/appveyor/ci/stephanmeissner/dccs-data-source.svg)

![NuGet Version](https://img.shields.io/nuget/v/DCCS.Data.Source.svg)

### Installing DCCS.Data.Source

You should install [DCCS.Data.Source with NuGet](https://www.nuget.org/packages/DCCS.Data.Source/):

    Install-Package DCCS.Data.Source

Or via the .NET Core command line interface:

    dotnet add package DCCS.Data.Source

Either commands, from Package Manager Console or .NET Core CLI, will download and install DCCS.Data.Source and all required dependencies.

### Usage

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
