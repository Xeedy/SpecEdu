namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service for GDPR compliance operations.
/// Handles data export, deletion, and anonymization requests.
/// </summary>
public interface IGdprService
{
    /// <summary>
    /// Exports all personal data for a user (GDPR Article 20 - Right to data portability).
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>JSON representation of all user data.</returns>
    Task<string> ExportUserDataAsync(string userId);

    /// <summary>
    /// Exports all data for a student (for guardian access or deletion request).
    /// </summary>
    /// <param name="studentId">The student's ID.</param>
    /// <returns>JSON representation of all student data.</returns>
    Task<string> ExportStudentDataAsync(Guid studentId);

    /// <summary>
    /// Anonymizes a user's data (GDPR Article 17 - Right to erasure).
    /// Replaces personal identifiers with anonymized values while maintaining audit trail.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>True if successful.</returns>
    Task<bool> AnonymizeUserDataAsync(string userId);

    /// <summary>
    /// Anonymizes a student's data while maintaining structure for reporting.
    /// </summary>
    /// <param name="studentId">The student's ID.</param>
    /// <returns>True if successful.</returns>
    Task<bool> AnonymizeStudentDataAsync(Guid studentId);

    /// <summary>
    /// Gets consent status for a user.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>Dictionary of consent types and their status.</returns>
    Task<IDictionary<string, bool>> GetConsentStatusAsync(string userId);

    /// <summary>
    /// Updates consent for a specific purpose.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="consentType">Type of consent (e.g., "marketing", "analytics").</param>
    /// <param name="granted">Whether consent is granted.</param>
    /// <returns>True if updated.</returns>
    Task<bool> UpdateConsentAsync(string userId, string consentType, bool granted);

    /// <summary>
    /// Gets data retention policy information for display.
    /// </summary>
    /// <returns>Dictionary of data types and their retention periods.</returns>
    IDictionary<string, string> GetDataRetentionPolicy();
}
