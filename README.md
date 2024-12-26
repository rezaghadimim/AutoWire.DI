# AutoWire.DI

AutoWire.DI is a lightweight dependency injection library for .NET applications that simplifies the process of registering services automatically. By utilizing attribute-based service configuration, AutoWire.DI helps developers reduce boilerplate code and ensures a clean and maintainable codebase.

## Features

- **Automatic Service Registration**: Automatically discovers and registers services decorated with the `AutoInject` attribute, minimizing manual setup.
- **Attribute-Based Configuration**: Allows developers to mark classes for injection with a simple attribute, enhancing readability.
- **Flexible Assembly Scanning**: Easily register services from specified assemblies, ensuring that all desired services are included.
- **Support for Keyed Registration**: Allows you to register services with specific keys, enabling the resolution of different implementations for the same service type based on the provided key.
- **Service Lifetime Management**: Supports different service lifetimes (e.g., Singleton, Transient, Scoped) for precise control over service instantiation. If no lifetime is specified, the default service lifetime is set to **Scoped**.
- **Seamless Integration**: Designed to work smoothly with Microsoft.Extensions.DependencyInjection, making integration into existing applications straightforward.

## Installation

You can install AutoWire.DI via NuGet Package Manager. Run the following command in your .NET project:

```bash
dotnet add package AutoWire.DI
```

## Usage

### 1. Define Your Services

Decorate your service classes with the AutoInject attribute to mark them for automatic registration. You can specify a key and the desired service lifetime. If no lifetime is specified, the service will default to Scoped .

```csharp
using AutoWire.DI.Attributes;

[AutoInject]
public class MyService : IMyService
{
    public void DoWork()
    {
        // Implementation
    }
}

[AutoInject("exampleKey", ServiceLifetime.Singleton)]
public class MyKeyedService : IMyService
{
    public void DoWork()
    {
        // Implementation
    }
}
```

### 2. Register Your Services

In your application's startup configuration, call the `RegisterAutoInjectableServices` method to automatically register services from the specified assembly.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register all services in the calling assembly that are marked with AutoInject
        services.RegisterAutoInjectableServices();
        
        // Register an alternative implementation with a different key
        services.AddTransient<IMyService, MyAlternativeService>("Alternative");
    }
}
```

#### Resolving Keyed Services
To resolve a service using its key, you can retrieve it from the service provider as shown:

```csharp
var serviceProvider = services.BuildServiceProvider();
var myService = serviceProvider.GetService<IMyService>("exampleKey");
var alternativeService = serviceProvider.GetService<IMyService>("Alternative");
```

### 3. Resolve Services in Your Application

You can then resolve your services using the built service provider.

```csharp
public class SomeController
{
    private readonly IMyService _myService;

    public SomeController(IMyService myService)
    {
        _myService = myService; // Automatically injected by the DI container
    }

    public void Execute()
    {
        _myService.DoWork();
    }
}

public class SomeController
{
    private readonly IMyService _myService;

    public SomeController([FromKeyedServices("exampleKey")] IMyService myService)
    {
        _myService = myService; // Automatically injected by the DI container
    }

    public void Execute()
    {
        _myService.DoWork();
    }
}
```

## Contributing

Contributions to AutoWire.DI are welcome! If you have suggestions, bug fixes, or improvements, please submit an issue or a pull request. We appreciate your help in making this project better!

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Thanks to the community for their continual support and contributions.
- Inspired by various dependency injection principles and frameworks in the .NET ecosystem.
