![CI](https://github.com/Toarnbeike/Toarnbeike.Dispatch/actions/workflows/build.yaml/badge.svg)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-blueviolet.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

# Toarnbeike.Dispatch
This package provides a **lightweight dispatching framework** for requests and notifications. 

It introduces **Request**, **RequestHandlers** and **Pipeline-behaviours**, inspired by the Mediator design pattern.

A request is an instruction for an underlying system to do something. Either change the state of the system (**Command**) or request a result (**Query**).
Each request type is handled by one specific handler, which uses the `Toarnbeike.Results` `Result` type to return either a success or a failed result.

Using the Mediator design pattern allowes for a decoupling of concerns, the request are a contract between client and server, whereas the handler is a server specific (internal) affair.

## Features

- **Result driven responses** - integrate directly with `Toarnbeike.Results` to make failures explicit.
- **Dependency injection integration** - configure using the provided DI extensions.
- **Strict Command and Query segregation** - Make intent clear from the definition of the request.
- **Pipeline-integration** - Use pipelines with request filters to abstract cross cutting concerns away from handlers.
- **Predefined CommandResponses** - Provide clear descriptions, with toaster integration, for successful commands.
- **Confirmation requering commands** - Create commands that require confirmation and reformatted requests after confirmation is given.

---

## Contents

1. [Quick start](#quick-start)
1. [Core concepts](#core-concepts)
1. [Pipelines](#pipelines)
1. [Responses](#responses)
1. [Conclusion](#conclusion)

---

## Quick start 

This example demonstrates the most common workflow regarding dispatching: registration, dispatching and handling the returned result.

``` csharp
using Toarnbeike.Requests;

public sealed record TestQuery(int Value) : IQuery<int>;									// Query that excepts an int and returns an int.

internal sealed class TestQueryHandler(IExternalDependency externalDependency) : IQueryHandler<TestQuery, int>															

// Query handler that handles the TestQuery and returns an int
{
	public async Task<Result<int>> HandleAsync(TestQuery request, CancellationToken cancellationToken = default)
	{
		return await externalDependency.IncrementByOne(request.Value);
	}
}
```

Clientside to work with the TestQueryHandler:

``` csharp
using Toarnbeike.Dispatch;
using Toarnbeike.Dispatch.DependencyInjection;

services.AddDispatching();
services.RegisterQueryHandler<TestQuery, int, TestQueryHandler>();			// starting from version 1.0, registration will be automatic using source generation.


var dispatcher = serviceProvider.GetRequiredService<IRequestDispatcher>(); // or inject via constructor.
Result<int> result = dispatcher.Dispatch(new TestQuery(42));

Console.WriteLine(result.Match(
	onSuccess: value => $"Success: returned value {value}",
	onFailure: failure => $"Something went wrong: {failure.Message}");

// output: Success: return value 43
```

## Core concepts

WIP

## Pipelines

WIP

## Responses

WIP

## Conclusion

> Decouple requests (contract) from handler logic (implementation)