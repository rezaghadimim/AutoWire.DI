using System.Reflection;
using AutoWire.DI.Attributes;
using AutoWire.DI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AutoWire.DI.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void RegisterAutoInjectableServices_ShouldRegisterAutoInjectServices()
    {
        // Arrange
        var serviceCollectionMock = new Mock<IServiceCollection>();
        var serviceDescriptors = new List<ServiceDescriptor>();

        // Mock IServiceCollection.Add
        serviceCollectionMock
            .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()))
            .Verifiable();

        // Setup GetEnumerator to return an empty list of service descriptors
        serviceCollectionMock.Setup(sc => sc.GetEnumerator())
            .Returns(serviceDescriptors.GetEnumerator());

        // Create mock for Assembly
        var assemblyMock = new Mock<Assembly>();

        // Mock GetTypes to only return a specific class type (e.g., IMyService)
        assemblyMock.Setup(a => a.GetTypes())
            .Returns([typeof(MyService)]); // Only return MyService as the type

        // Setup RegisterAutoInjectableServices to use the mocked assembly
        var serviceCollection = serviceCollectionMock.Object;

        // Act
        serviceCollection.RegisterAutoInjectableServices(); // Call the method under test

        // Assert
        // Verify that the Add method was called at least once, indicating services were registered
        serviceCollectionMock.Verify(sc => sc.Add(It.IsAny<ServiceDescriptor>()), Times.AtLeastOnce);

        // You can also check if the MyService class has been added to the service descriptors
        Assert.Contains(serviceDescriptors, sd => sd.ServiceType == typeof(MyService)); // Ensure MyService is registered
    }

    // Sample service interface
    private interface IMyService;

    // Sample service implementation
    [AutoInject]
    private class MyService : IMyService;
}
