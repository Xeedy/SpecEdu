using Microsoft.Extensions.Logging;

namespace SpecEdu.Web.Logging;

/// <summary>
/// Centralized logging for SpecEdu application.
/// Uses LoggerMessage source generators for high-performance structured logging.
///
/// Event ID Ranges:
/// - 1000-1999: Database operations
/// - 2000-2999: Authentication
/// - 3000-3999: Authorization
/// - 4000-4999: Application/Business logic
/// - 5000-5999: Infrastructure/External services
/// - 9000-9999: System/Startup
/// </summary>
internal static partial class Log
{
    // ============================================
    // 1000-1999: DATABASE OPERATIONS
    // ============================================

    [LoggerMessage(EventId = 1000, Level = LogLevel.Information, Message = "Database connection established successfully")]
    public static partial void DatabaseConnected(ILogger logger);

    [LoggerMessage(EventId = 1001, Level = LogLevel.Error, Message = "Database connection failed: {errorMessage}")]
    public static partial void DatabaseConnectionFailed(ILogger logger, string errorMessage);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Information, Message = "Entity {entityType} with ID {entityId} created")]
    public static partial void EntityCreated(ILogger logger, string entityType, string entityId);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Information, Message = "Entity {entityType} with ID {entityId} updated")]
    public static partial void EntityUpdated(ILogger logger, string entityType, string entityId);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Information, Message = "Entity {entityType} with ID {entityId} deleted")]
    public static partial void EntityDeleted(ILogger logger, string entityType, string entityId);

    [LoggerMessage(EventId = 1005, Level = LogLevel.Warning, Message = "Entity {entityType} with ID {entityId} not found")]
    public static partial void EntityNotFound(ILogger logger, string entityType, string entityId);

    [LoggerMessage(EventId = 1010, Level = LogLevel.Information, Message = "Database migration started")]
    public static partial void MigrationStarted(ILogger logger);

    [LoggerMessage(EventId = 1011, Level = LogLevel.Information, Message = "Database migration completed successfully")]
    public static partial void MigrationCompleted(ILogger logger);

    [LoggerMessage(EventId = 1012, Level = LogLevel.Error, Message = "Database migration failed: {errorMessage}")]
    public static partial void MigrationFailed(ILogger logger, string errorMessage);

    // ============================================
    // 2000-2999: AUTHENTICATION
    // ============================================

    [LoggerMessage(EventId = 2000, Level = LogLevel.Information, Message = "User {email} login attempt")]
    public static partial void LoginAttempt(ILogger logger, string email);

    [LoggerMessage(EventId = 2001, Level = LogLevel.Information, Message = "User {email} logged in successfully")]
    public static partial void LoginSuccess(ILogger logger, string email);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Warning, Message = "User {email} login failed: {reason}")]
    public static partial void LoginFailed(ILogger logger, string email, string reason);

    [LoggerMessage(EventId = 2003, Level = LogLevel.Information, Message = "User {email} logged out")]
    public static partial void Logout(ILogger logger, string email);

    [LoggerMessage(EventId = 2010, Level = LogLevel.Information, Message = "User {email} registered successfully")]
    public static partial void UserRegistered(ILogger logger, string email);

    [LoggerMessage(EventId = 2011, Level = LogLevel.Warning, Message = "User registration failed for {email}: {reason}")]
    public static partial void UserRegistrationFailed(ILogger logger, string email, string reason);

    [LoggerMessage(EventId = 2020, Level = LogLevel.Information, Message = "Password reset requested for {email}")]
    public static partial void PasswordResetRequested(ILogger logger, string email);

    [LoggerMessage(EventId = 2021, Level = LogLevel.Information, Message = "Password reset completed for {email}")]
    public static partial void PasswordResetCompleted(ILogger logger, string email);

    [LoggerMessage(EventId = 2030, Level = LogLevel.Information, Message = "JWT token generated for user {userId}")]
    public static partial void TokenGenerated(ILogger logger, string userId);

    [LoggerMessage(EventId = 2031, Level = LogLevel.Warning, Message = "JWT token validation failed: {reason}")]
    public static partial void TokenValidationFailed(ILogger logger, string reason);

    [LoggerMessage(EventId = 2032, Level = LogLevel.Information, Message = "JWT token refreshed for user {userId}")]
    public static partial void TokenRefreshed(ILogger logger, string userId);

    [LoggerMessage(EventId = 2040, Level = LogLevel.Warning, Message = "Account locked for user {email} after {attempts} failed attempts")]
    public static partial void AccountLocked(ILogger logger, string email, int attempts);

    // ============================================
    // 3000-3999: AUTHORIZATION
    // ============================================

    [LoggerMessage(EventId = 3000, Level = LogLevel.Warning, Message = "Access denied for user {userId} to resource {resource}")]
    public static partial void AccessDenied(ILogger logger, string userId, string resource);

    [LoggerMessage(EventId = 3001, Level = LogLevel.Information, Message = "User {userId} granted access to resource {resource}")]
    public static partial void AccessGranted(ILogger logger, string userId, string resource);

    [LoggerMessage(EventId = 3010, Level = LogLevel.Information, Message = "Role {role} assigned to user {userId}")]
    public static partial void RoleAssigned(ILogger logger, string role, string userId);

    [LoggerMessage(EventId = 3011, Level = LogLevel.Information, Message = "Role {role} removed from user {userId}")]
    public static partial void RoleRemoved(ILogger logger, string role, string userId);

    [LoggerMessage(EventId = 3020, Level = LogLevel.Warning, Message = "Insufficient permissions for user {userId}: required {requiredRole}, has {userRoles}")]
    public static partial void InsufficientPermissions(ILogger logger, string userId, string requiredRole, string userRoles);

    // ============================================
    // 4000-4999: APPLICATION/BUSINESS LOGIC
    // ============================================

    // Student operations (4000-4099)
    [LoggerMessage(EventId = 4000, Level = LogLevel.Information, Message = "Student {studentId} profile created by user {userId}")]
    public static partial void StudentCreated(ILogger logger, string studentId, string userId);

    [LoggerMessage(EventId = 4001, Level = LogLevel.Information, Message = "Student {studentId} profile updated by user {userId}")]
    public static partial void StudentUpdated(ILogger logger, string studentId, string userId);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Information, Message = "Student {studentId} profile viewed by user {userId}")]
    public static partial void StudentViewed(ILogger logger, string studentId, string userId);

    // Document operations (4100-4199)
    [LoggerMessage(EventId = 4100, Level = LogLevel.Information, Message = "Document {documentId} uploaded for student {studentId} by user {userId}")]
    public static partial void DocumentUploaded(ILogger logger, string documentId, string studentId, string userId);

    [LoggerMessage(EventId = 4101, Level = LogLevel.Information, Message = "Document {documentId} downloaded by user {userId}")]
    public static partial void DocumentDownloaded(ILogger logger, string documentId, string userId);

    [LoggerMessage(EventId = 4102, Level = LogLevel.Information, Message = "Document {documentId} deleted by user {userId}")]
    public static partial void DocumentDeleted(ILogger logger, string documentId, string userId);

    // IVP/PLPP operations (4200-4299)
    [LoggerMessage(EventId = 4200, Level = LogLevel.Information, Message = "IVP created for student {studentId} by user {userId}")]
    public static partial void IvpCreated(ILogger logger, string studentId, string userId);

    [LoggerMessage(EventId = 4201, Level = LogLevel.Information, Message = "IVP updated for student {studentId} by user {userId}")]
    public static partial void IvpUpdated(ILogger logger, string studentId, string userId);

    [LoggerMessage(EventId = 4210, Level = LogLevel.Information, Message = "PLPP created for student {studentId} by user {userId}")]
    public static partial void PlppCreated(ILogger logger, string studentId, string userId);

    [LoggerMessage(EventId = 4211, Level = LogLevel.Information, Message = "PLPP updated for student {studentId} by user {userId}")]
    public static partial void PlppUpdated(ILogger logger, string studentId, string userId);

    // Communication (4300-4399)
    [LoggerMessage(EventId = 4300, Level = LogLevel.Information, Message = "Note added to student {studentId} by user {userId}")]
    public static partial void NoteAdded(ILogger logger, string studentId, string userId);

    [LoggerMessage(EventId = 4310, Level = LogLevel.Information, Message = "Consultation scheduled for student {studentId} on {date}")]
    public static partial void ConsultationScheduled(ILogger logger, string studentId, string date);

    // ============================================
    // 5000-5999: INFRASTRUCTURE/EXTERNAL SERVICES
    // ============================================

    [LoggerMessage(EventId = 5000, Level = LogLevel.Information, Message = "Email sent to {recipient}: {subject}")]
    public static partial void EmailSent(ILogger logger, string recipient, string subject);

    [LoggerMessage(EventId = 5001, Level = LogLevel.Error, Message = "Email sending failed to {recipient}: {errorMessage}")]
    public static partial void EmailFailed(ILogger logger, string recipient, string errorMessage);

    [LoggerMessage(EventId = 5010, Level = LogLevel.Information, Message = "File {fileName} stored successfully, size: {fileSize} bytes")]
    public static partial void FileStored(ILogger logger, string fileName, long fileSize);

    [LoggerMessage(EventId = 5011, Level = LogLevel.Error, Message = "File storage failed for {fileName}: {errorMessage}")]
    public static partial void FileStorageFailed(ILogger logger, string fileName, string errorMessage);

    [LoggerMessage(EventId = 5020, Level = LogLevel.Information, Message = "External service {serviceName} called successfully")]
    public static partial void ExternalServiceSuccess(ILogger logger, string serviceName);

    [LoggerMessage(EventId = 5021, Level = LogLevel.Error, Message = "External service {serviceName} call failed: {errorMessage}")]
    public static partial void ExternalServiceFailed(ILogger logger, string serviceName, string errorMessage);

    // ============================================
    // 9000-9999: SYSTEM/STARTUP
    // ============================================

    [LoggerMessage(EventId = 9000, Level = LogLevel.Information, Message = "Application starting, version {version}")]
    public static partial void ApplicationStarting(ILogger logger, string version);

    [LoggerMessage(EventId = 9001, Level = LogLevel.Information, Message = "Application started successfully")]
    public static partial void ApplicationStarted(ILogger logger);

    [LoggerMessage(EventId = 9002, Level = LogLevel.Information, Message = "Application shutting down")]
    public static partial void ApplicationShuttingDown(ILogger logger);

    [LoggerMessage(EventId = 9010, Level = LogLevel.Information, Message = "Configuration loaded: {configSection}")]
    public static partial void ConfigurationLoaded(ILogger logger, string configSection);

    [LoggerMessage(EventId = 9011, Level = LogLevel.Warning, Message = "Configuration missing: {configKey}")]
    public static partial void ConfigurationMissing(ILogger logger, string configKey);

    [LoggerMessage(EventId = 9020, Level = LogLevel.Information, Message = "Health check passed: {checkName}")]
    public static partial void HealthCheckPassed(ILogger logger, string checkName);

    [LoggerMessage(EventId = 9021, Level = LogLevel.Warning, Message = "Health check failed: {checkName}, reason: {reason}")]
    public static partial void HealthCheckFailed(ILogger logger, string checkName, string reason);

    [LoggerMessage(EventId = 9030, Level = LogLevel.Error, Message = "Unhandled exception: {exceptionType} - {message}")]
    public static partial void UnhandledException(ILogger logger, string exceptionType, string message);
}
