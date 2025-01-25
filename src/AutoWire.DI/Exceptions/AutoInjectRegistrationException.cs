namespace AutoWire.DI.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an error occurs during the automatic registration of services.
/// </summary>
public class AutoInjectRegistrationException(string message) : Exception(message);
