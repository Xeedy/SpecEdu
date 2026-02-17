using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

public class UserConsent : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string ConsentType { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
}
