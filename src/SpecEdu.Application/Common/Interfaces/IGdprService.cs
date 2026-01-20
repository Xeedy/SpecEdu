namespace SpecEdu.Application.Common.Interfaces;

public interface IGdprService
{
    Task<string> ExportUserDataAsync(string userId);

    Task<string> ExportStudentDataAsync(Guid studentId);

    Task<bool> AnonymizeUserDataAsync(string userId);

    Task<bool> AnonymizeStudentDataAsync(Guid studentId);

    Task<IDictionary<string, bool>> GetConsentStatusAsync(string userId);

    Task<bool> UpdateConsentAsync(string userId, string consentType, bool granted);

    IDictionary<string, string> GetDataRetentionPolicy();
}
