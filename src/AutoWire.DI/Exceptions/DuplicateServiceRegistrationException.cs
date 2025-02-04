namespace AutoWire.DI.Exceptions;

/// <summary>
/// Exception thrown when attempting to register a duplicate service with the same type and key.
/// </summary>
public class DuplicateServiceRegistrationException : AutoInjectRegistrationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateServiceRegistrationException"/> class.
    /// </summary>
    /// <param name="newType">The new class that is trying to be registered.</param>
    /// <param name="existingType">The already registered class that conflicts.</param>
    /// <param name="serviceType">The service type that caused the conflict.</param>
    /// <param name="key">The optional key associated with the service.</param>
    public DuplicateServiceRegistrationException(Type newType,
        Type existingType,
        Type serviceType,
        string? key)
        : base($"A conflict occurred while registering services. The class '{newType.FullName}' is trying to register " +
               $"as '{serviceType.FullName}' with key '{key ?? "null"}', but it conflicts with an existing registration: " +
               $"'{existingType.FullName}'. Duplicate registrations are not allowed.")
    {
    }
}
