namespace AutoWire.DI.Exceptions;

/// <summary>
/// Exception thrown when a class implements multiple interfaces but no explicit service type is specified.
/// </summary>
public class AmbiguousServiceTypeException : AutoInjectRegistrationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmbiguousServiceTypeException"/> class.
    /// </summary>
    /// <param name="className">The name of the class that caused the exception.</param>
    public AmbiguousServiceTypeException(string className)
        : base($"The class '{className}' implements multiple interfaces, but no explicit service type is specified in [AutoInject]. " +
               "Please provide a service type to avoid ambiguity.")
    {
    }
}
