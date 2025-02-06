# AutoWire.DI

[![NuGet Version](https://img.shields.io/nuget/v/AutoWire.DI.svg)](https://www.nuget.org/packages/AutoWire.DI/)

**AutoWire.DI** is a powerful, lightweight **dependency injection** (DI) library for **.NET** applications featuring automatic registration capabilities. This library simplifies the process of injecting dependencies into your classes by automating service registration, allowing you to focus more on your application logic without the boilerplate associated with manual DI setup.

## Key Features

- **Automatic Service Registration**: Effortlessly register services with minimal configuration using the `[AutoInject]` attribute.
- **Flexible Service Lifetimes**: Support for `Singleton`, `Scoped`, and `Transient` lifetimes.
- **No Manual Configuration**: Forget about manually adding each service to your container—just decorate your classes.
- **Error-Free Registration**: Built-in exception handling for duplicate and ambiguous registrations, ensuring a smooth DI experience.
- **Support for .NET 8.0 and 9.0**: Fully compatible with the latest versions of .NET.

## Getting Started

### Installation

To install `AutoWire.DI` into your project, run the following command in your terminal or package manager console:

```sh
dotnet add package AutoWire.DI
```

### Simple Usage

1. **Define Your Service Class**

Simply add the `[AutoInject]` attribute to your class, and it will be automatically registered in the DI container. You can also specify the service lifetime and service type.

```csharp
using AutoWire.DI.Attributes;

[AutoInject("myServiceKey", ServiceLifetime.Singleton)]
public class MyService
{
    // Service implementation details
}
```

- **No Parameters? No Problem!**
  If no parameters are specified in the `[AutoInject]` attribute, the service is registered based on its implemented interface or class type.

```csharp
[AutoInject]
public class MyService
{
    // Service implementation
}
```

2. **Register Services in Your DI Container**

In your `Startup.cs` (or `Program.cs` if you're using .NET 6+), call the `RegisterAutoInjectableServices` method to automatically scan and register all services decorated with `[AutoInject]`.

```csharp
using AutoWire.DI.Extensions;
using Microsoft.Extensions.DependencyInjection;

public void ConfigureServices(IServiceCollection services)
{
    services.RegisterAutoInjectableServices();
}
```

### Exception Handling

- **AmbiguousServiceTypeException**: This exception is thrown when a class implements multiple interfaces but doesn't specify which one to use. To avoid this, always specify the service type in such cases.
- **DuplicateServiceRegistrationException**: This exception is thrown if a service is being registered with the same service type and key, leading to a conflict. Ensure that you don't have conflicting registrations.

## Why Choose AutoWire.DI?

- **Simplify Dependency Injection**: Get rid of repetitive DI configuration and make your code cleaner and more maintainable.
- **Automatic and Flexible**: Works seamlessly with both simple services and more complex configurations.
- **Error-Free Registration**: With built-in checks for duplicate and ambiguous registrations, you avoid the common pitfalls of manual DI setup.

## Documentation

For more in-depth guidance on using **AutoWire.DI** and advanced configurations, check out the full documentation on our [GitHub Wiki](https://github.com/rezaghadimim/AutoWire.DI/wiki).

## Contributing

Contributions to AutoWire.DI are welcome! If you have suggestions, bug fixes, or improvements, please submit an issue or a pull request. We appreciate your help in making this project better!

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.


## Acknowledgments

Inspired by various dependency injection principles and frameworks in the .NET ecosystem.
