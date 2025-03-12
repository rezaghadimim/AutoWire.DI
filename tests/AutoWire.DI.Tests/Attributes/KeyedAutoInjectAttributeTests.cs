using AutoWire.DI.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Tests.Attributes;

public class KeyedAutoInjectAttributeTests
{
    [Fact]
    public void KeyedAutoInjectAttribute_Should_Set_Lifetime_And_Key()
    {
        // Arrange
        var type = typeof(ExampleClass);
        var attribute = (KeyedAutoInjectAttribute)Attribute.GetCustomAttribute(type, typeof(KeyedAutoInjectAttribute))!;

        // Act & Assert
        Assert.NotNull(attribute);
        Assert.Equal(ServiceLifetime.Singleton, attribute.Lifetime);
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void KeyedAutoInjectAttribute_Default_Should_Be_Scoped_And__Key()
    {
        // Arrange
        var noKeyClassType = typeof(DefaultScopeExampleClass);
        var attribute = (KeyedAutoInjectAttribute)Attribute.GetCustomAttribute(noKeyClassType, typeof(KeyedAutoInjectAttribute))!;

        // Act & Assert
        Assert.NotNull(attribute);
        Assert.Equal(ServiceLifetime.Scoped, attribute.Lifetime);
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void KeyedAutoInjectAttribute_Should_Not_Be_Inherited()
    {
        // Arrange
        var inheritedClassType = typeof(InheritedExampleClass);
        var attribute = Attribute.GetCustomAttribute(inheritedClassType, typeof(KeyedAutoInjectAttribute));

        // Act & Assert
        Assert.Null(attribute);
    }

    [Fact]
    public void KeyedAutoInjectAttribute_Only_ServiceType_Specified()
    {
        // Arrange
        var onlyServiceTypeSpecifiedClassType = typeof(OnlyServiceTypeSpecifiedClass);
        var attribute =
            (KeyedAutoInjectAttribute)Attribute.GetCustomAttribute(onlyServiceTypeSpecifiedClassType, typeof(KeyedAutoInjectAttribute))!;

        // Act & Assert
        Assert.NotNull(attribute);
        Assert.Equal(typeof(OnlyServiceTypeSpecifiedClass), attribute.ServiceType);
        Assert.Null(attribute.Key);
    }

    [KeyedAutoInject(ServiceLifetime.Singleton)]
    private class ExampleClass;

    [KeyedAutoInject]
    private class DefaultScopeExampleClass;

    private class InheritedExampleClass : ExampleClass;

    private interface IOnlyServiceTypeSpecifiedClass;

    [KeyedAutoInject(serviceType: typeof(OnlyServiceTypeSpecifiedClass))]
    private class OnlyServiceTypeSpecifiedClass : IOnlyServiceTypeSpecifiedClass;
}
