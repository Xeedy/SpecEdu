namespace SpecEdu.Domain.Enums;

public enum NotificationType
{
    /// <summary>
    /// General information notification.
    /// Czech: Obecná informace
    /// </summary>
    Info = 1,

    /// <summary>
    /// New diary entry notification.
    /// Czech: Nový záznam v deníku
    /// </summary>
    DiaryEntry = 2,

    /// <summary>
    /// PLPP update notification.
    /// Czech: Aktualizace PLPP
    /// </summary>
    PlppUpdate = 3,

    /// <summary>
    /// Consultation invitation notification.
    /// Czech: Pozvánka na konzultaci
    /// </summary>
    ConsultationInvite = 4,

    /// <summary>
    /// Reminder due notification.
    /// Czech: Nadcházející připomínka
    /// </summary>
    ReminderDue = 5,

    /// <summary>
    /// System-wide announcement.
    /// Czech: Systémové oznámení
    /// </summary>
    SystemAnnouncement = 6
}
