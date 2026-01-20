namespace SpecEdu.Application.Common.Exceptions;

/// <summary>
/// Base exception for business logic errors.
/// Should be caught and handled gracefully in the presentation layer.
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// Error code for client identification.
    /// </summary>
    public string ErrorCode { get; }

    public BusinessException(string message, string errorCode = "BUSINESS_ERROR")
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public BusinessException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
public class EntityNotFoundException : BusinessException
{
    public string EntityType { get; }
    public object EntityId { get; }

    public EntityNotFoundException(string entityType, object entityId)
        : base($"{entityType} with ID '{entityId}' was not found.", "ENTITY_NOT_FOUND")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : BusinessException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message)
        : base(message, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.", "VALIDATION_ERROR")
    {
        Errors = errors;
    }
}

/// <summary>
/// Exception thrown when access to a resource is denied.
/// </summary>
public class AccessDeniedException : BusinessException
{
    public string Resource { get; }
    public string UserId { get; }

    public AccessDeniedException(string resource, string userId)
        : base($"Access to '{resource}' is denied for user '{userId}'.", "ACCESS_DENIED")
    {
        Resource = resource;
        UserId = userId;
    }
}
