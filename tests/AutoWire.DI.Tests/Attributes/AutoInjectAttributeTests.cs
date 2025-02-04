using AutoWire.DI.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Tests.Attributes;

public class AutoInjectAttributeTests
{
    [Fact]
    public void AutoInjectAttribute_Should_Set_Lifetime_And_Key()
    {
        // Arrange
        var type = typeof(ExampleClass);
        var attribute = (AutoInjectAttribute)Attribute.GetCustomAttribute(type, typeof(AutoInjectAttribute))!;

        // Act & Assert
        Assert.NotNull(attribute);
        Assert.Equal(ServiceLifetime.Singleton, attribute.Lifetime);
        Assert.Equal("exampleKey", attribute.Key);
    }

    [Fact]
    public void AutoInjectAttribute_Default_Should_Be_Scoped_And_Null_Key()
    {
        // Arrange
        var noKeyClassType = typeof(NoKeyExampleClass);
        var attribute = (AutoInjectAttribute)Attribute.GetCustomAttribute(noKeyClassType, typeof(AutoInjectAttribute))!;

        // Act & Assert
        Assert.NotNull(attribute);
        Assert.Equal(ServiceLifetime.Scoped, attribute.Lifetime);
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void AutoInjectAttribute_Should_Not_Be_AutoInjected()
    {
        // Arrange
        var notAutoInjectClassType = typeof(NotAutoInjectClass);
        var attribute = Attribute.GetCustomAttribute(notAutoInjectClassType, typeof(AutoInjectAttribute));

        // Act & Assert
        Assert.Null(attribute);
    }

    [Fact]
    public void AutoInjectAttribute_Should_Not_Be_Inherited()
    {
        // Arrange
        var inheritedClassType = typeof(InheritedExampleClass);
        var attribute = Attribute.GetCustomAttribute(inheritedClassType, typeof(AutoInjectAttribute));

        // Act & Assert
        Assert.Null(attribute);
    }

    [Fact]
    public void AutoInjectAttribute_Only_Lifetime_Specified()
    {
        // Arrange
        var onlyLifetimeSpecifiedClassType = typeof(OnlyLifetimeSpecifiedClass);
        var attribute = (AutoInjectAttribute)Attribute.GetCustomAttribute(onlyLifetimeSpecifiedClassType, typeof(AutoInjectAttribute))!;

        // Act & Assert
        Assert.NotNull(attribute);
        Assert.Equal(ServiceLifetime.Transient, attribute.Lifetime);
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void AutoInjectAttribute_Only_Key_Specified()
    {
        // Arrange
        var onlyKeySpecifiedClassType = typeof(OnlyKeySpecifiedClass);
        var attribute = (AutoInjectAttribute)Attribute.GetCustomAttribute(onlyKeySpecifiedClassType, typeof(AutoInjectAttribute))!;

        // Act & Assert
        Assert.NotNull(attribute);
        Assert.Equal(ServiceLifetime.Scoped, attribute.Lifetime);
        Assert.Equal("exampleKey", attribute.Key);
    }

    [Fact]
    public void AutoInjectAttribute_Only_ServiceType_Specified()
    {
        // Arrange
        var onlyServiceTypeSpecifiedClassType = typeof(OnlyServiceTypeSpecifiedClass);
        var attribute = (AutoInjectAttribute)Attribute.GetCustomAttribute(onlyServiceTypeSpecifiedClassType, typeof(AutoInjectAttribute))!;

        // Act & Assert
        Assert.NotNull(attribute);
        Assert.Equal(typeof(OnlyServiceTypeSpecifiedClass), attribute.ServiceType);
    }

    [AutoInject("exampleKey", ServiceLifetime.Singleton)]
    private class ExampleClass;

    [AutoInject]
    private class NoKeyExampleClass;

    private class NotAutoInjectClass;

    private class InheritedExampleClass : ExampleClass;

    [AutoInject(lifetime: ServiceLifetime.Transient)]
    private class OnlyLifetimeSpecifiedClass;

    [AutoInject("exampleKey")]
    private class OnlyKeySpecifiedClass;

    private interface IOnlyServiceTypeSpecifiedClass;

    [AutoInject(serviceType: typeof(OnlyServiceTypeSpecifiedClass))]
    private class OnlyServiceTypeSpecifiedClass : IOnlyServiceTypeSpecifiedClass;
}
