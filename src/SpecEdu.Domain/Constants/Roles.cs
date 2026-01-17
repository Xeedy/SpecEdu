namespace SpecEdu.Domain.Constants;

/// <summary>
/// Defines all user roles in the SpecEdu system.
/// These roles control access to different features and data.
/// </summary>
public static class Roles
{
    /// <summary>
    /// Administrator role - full system access, one per school.
    /// Czech: Správce
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// Teacher/Pedagogue role - manages students and documentation.
    /// Czech: Pedagog, Učitel
    /// </summary>
    public const string Teacher = "Teacher";

    /// <summary>
    /// Parent role - view-only access to their own child's profile.
    /// Czech: Rodič
    /// </summary>
    public const string Parent = "Parent";

    /// <summary>
    /// School counseling center role.
    /// Czech: ŠPP (Školní poradenské pracoviště)
    /// </summary>
    public const string SPP = "SPP";

    /// <summary>
    /// Pedagogical-psychological counseling role.
    /// Czech: PPP (Pedagogicko-psychologická poradna)
    /// </summary>
    public const string PPP = "PPP";

    /// <summary>
    /// Special pedagogical center role.
    /// Czech: SPC (Speciálně pedagogické centrum)
    /// </summary>
    public const string SPC = "SPC";

    /// <summary>
    /// Teaching assistant role.
    /// Czech: Asistent
    /// </summary>
    public const string Assistant = "Assistant";

    /// <summary>
    /// Gets all available roles as an array.
    /// Useful for seeding and validation.
    /// </summary>
    public static readonly string[] All =
    [
        Admin,
        Teacher,
        Parent,
        SPP,
        PPP,
        SPC,
        Assistant
    ];
}
