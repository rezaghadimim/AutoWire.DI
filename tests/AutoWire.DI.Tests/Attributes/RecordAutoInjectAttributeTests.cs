using AutoWire.DI.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Tests.Attributes;

public class RecordAutoInjectAttributeTests
{
    [Fact]
    public void AutoInjectAttribute_Should_Set_Lifetime_And_Key()
    {
        // Arrange
        var type = typeof(ExampleRecord);
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
        var noKeyRecordType = typeof(NoKeyExampleRecord);
        var attribute = (AutoInjectAttribute)Attribute.GetCustomAttribute(noKeyRecordType, typeof(AutoInjectAttribute))!;

        // Act & Assert
        Assert.NotNull(attribute);
        Assert.Equal(ServiceLifetime.Scoped, attribute.Lifetime);
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void AutoInjectAttribute_Should_Not_Be_AutoInjected()
    {
        // Arrange
        var notAutoInjectRecordType = typeof(NotAutoInjectRecord);
        var attribute = Attribute.GetCustomAttribute(notAutoInjectRecordType, typeof(AutoInjectAttribute));

        // Act & Assert
        Assert.Null(attribute);
    }

    [Fact]
    public void AutoInjectAttribute_Should_Not_Be_Inherited()
    {
        // Arrange
        var inheritedRecordType = typeof(InheritedExampleRecord);
        var attribute = Attribute.GetCustomAttribute(inheritedRecordType, typeof(AutoInjectAttribute));

        // Act & Assert
        Assert.Null(attribute);
    }

    [Fact]
    public void AutoInjectAttribute_Only_Lifetime_Specified()
    {
        // Arrange
        var onlyLifetimeSpecifiedRecordType = typeof(OnlyLifetimeSpecifiedRecord);
        var attribute = (AutoInjectAttribute)Attribute.GetCustomAttribute(onlyLifetimeSpecifiedRecordType, typeof(AutoInjectAttribute))!;

        // Act & Assert
        Assert.NotNull(attribute);
        Assert.Equal(ServiceLifetime.Transient, attribute.Lifetime);
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void AutoInjectAttribute_Only_Key_Specified()
    {
        // Arrange
        var onlyKeySpecifiedRecordType = typeof(OnlyKeySpecifiedRecord);
        var attribute = (AutoInjectAttribute)Attribute.GetCustomAttribute(onlyKeySpecifiedRecordType, typeof(AutoInjectAttribute))!;

        // Act & Assert
        Assert.NotNull(attribute);
        Assert.Equal(ServiceLifetime.Scoped, attribute.Lifetime);
        Assert.Equal("exampleKey", attribute.Key);
    }

    [AutoInject("exampleKey", ServiceLifetime.Singleton)]
    private record ExampleRecord;

    [AutoInject]
    private record NoKeyExampleRecord;

    private record NotAutoInjectRecord;

    private record InheritedExampleRecord : ExampleRecord;

    [AutoInject(lifetime: ServiceLifetime.Transient)]
    private record OnlyLifetimeSpecifiedRecord;

    [AutoInject("exampleKey")]
    private record OnlyKeySpecifiedRecord;
}
