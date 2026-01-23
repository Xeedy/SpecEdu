namespace SpecEdu.Domain.Enums;

/// <summary>
/// Type of consultation event.
/// Czech: Typ konzultace
/// </summary>
public enum ConsultationType
{
    /// <summary>
    /// Regular parent-teacher meeting.
    /// Czech: Třídní schůzka
    /// </summary>
    ParentTeacherMeeting = 1,

    /// <summary>
    /// Individual consultation about a specific student.
    /// Czech: Individuální konzultace
    /// </summary>
    IndividualConsultation = 2,

    /// <summary>
    /// PLPP review meeting.
    /// Czech: Vyhodnocení PLPP
    /// </summary>
    PlppReview = 3,

    /// <summary>
    /// IVP (Individual Education Plan) review.
    /// Czech: Vyhodnocení IVP
    /// </summary>
    IvpReview = 4,

    /// <summary>
    /// Meeting with school counselor or psychologist.
    /// Czech: Schůzka se školním poradenským pracovištěm
    /// </summary>
    CounselorMeeting = 5,

    /// <summary>
    /// Meeting with external specialist (PPP, SPC).
    /// Czech: Schůzka s externím specialistou
    /// </summary>
    ExternalSpecialistMeeting = 6,

    /// <summary>
    /// School event (open day, performance, etc.).
    /// Czech: Školní akce
    /// </summary>
    SchoolEvent = 7,

    /// <summary>
    /// Other type of consultation.
    /// Czech: Jiné
    /// </summary>
    Other = 99
}
