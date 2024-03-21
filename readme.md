# Immediate.Apis

[![NuGet](https://img.shields.io/nuget/v/Immediate.Apis.svg?style=plastic)](https://www.nuget.org/packages/Immediate.Apis/)
[![GitHub release](https://img.shields.io/github/release/viceroypenguin/Immediate.Apis.svg)](https://GitHub.com/viceroypenguin/Immediate.Apis/releases/)
[![GitHub license](https://img.shields.io/github/license/viceroypenguin/Immediate.Apis.svg)](https://github.com/viceroypenguin/Immediate.Apis/blob/master/license.txt) 
[![GitHub issues](https://img.shields.io/github/issues/viceroypenguin/Immediate.Apis.svg)](https://GitHub.com/viceroypenguin/Immediate.Apis/issues/) 
[![GitHub issues-closed](https://img.shields.io/github/issues-closed/viceroypenguin/Immediate.Apis.svg)](https://GitHub.com/viceroypenguin/Immediate.Apis/issues?q=is%3Aissue+is%3Aclosed) 
[![GitHub Actions](https://github.com/viceroypenguin/Immediate.Apis/actions/workflows/build.yml/badge.svg)](https://github.com/viceroypenguin/Immediate.Apis/actions)
---

Immediate.Apis is a source generator for minimal APIs, for
[`Immediate.Handlers`](https://github.com/viceroypenguin/Immediate.Handlers) handlers. Simply add a `[MapGet]` to the
`[Handler]` class and an endpoint will automatically be added.

#### Examples
* [Immediate.Apis.FunctionalTests](./tests/Immediate.Apis.FunctionalTests)

## Installing Immediate.Apis

You can install [Immediate.Apis with NuGet](https://www.nuget.org/packages/Immediate.Apis):

    Install-Package Immediate.Apis
    
Or via the .NET Core command line interface:

    dotnet add package Immediate.Apis

Either commands, from Package Manager Console or .NET Core CLI, will download and install Immediate.Handlers.

## Using Immediate.Apis
### Creating an Endpoint

Create a Handler and an endpoint by adding the following code:

```cs
[Handler]
[MapGet("/users")]
public static partial class GetUsersQuery
{
    public record Query;

    private static ValueTask<IEnumerable<User>> HandleAsync(
        Query _,
        UsersService usersService,
        CancellationToken token)
    {
        return usersService.GetUsers();
    }
}
```

### Registering the endpoints

In your `Program.cs`, add a call to `app.MapXxxEndpoints()`, where `Xxx` is the shortened form of the project name.
* For a project named `Web`, it will be `app.MapWebEndpoints()`
* For a project named `Application.Web`, it will be `app.MapApplicationWebEndpoints()`

### Customizing the endpoints
#### Authorization

The `[AllowAnonymous]` and `[Authorized("Policy")]` attributes are supported and will be applied to the endpoint.

```cs
[Handler]
[MapGet("/users")]
[AllowAnonymous]
public static partial class GetUsersQuery
{
    public record Query;

    private static ValueTask<IEnumerable<User>> HandleAsync(
        Query _,
        UsersService usersService,
        CancellationToken token)
    {
        return usersService.GetUsers();
    }
}
```

#### Additional Customization

Additional customization of the endpoint registration can be done by adding a `CustomizeEndpoint` method.

```cs
[Handler]
[MapGet("/users")]
[Authorize(Policies.UserManagement)]
public static partial class GetUsersQuery
{
    internal static void CustomizeGetFeaturesEndpoint(IEndpointConventionBuilder endpoint)
        => endpoint
            .Produces<IEnumerable<User>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags(nameof(User));

    public record Query;

    private static ValueTask<IEnumerable<User>> HandleAsync(
        Query _,
        UsersService usersService,
        CancellationToken token)
    {
        return usersService.GetUsers();
    }
}
```
