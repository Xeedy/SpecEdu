namespace SpecEdu.Domain.Enums;

/// <summary>
/// Defines the channel through which a notification is sent.
/// Czech: Kanál pro odesílání notifikací
/// </summary>
public enum NotificationChannel
{
    /// <summary>
    /// Send notification via email.
    /// Czech: E-mail
    /// </summary>
    Email = 1,

    /// <summary>
    /// In-app notification (bell icon).
    /// Czech: Notifikace v aplikaci
    /// </summary>
    InApp = 2
}
