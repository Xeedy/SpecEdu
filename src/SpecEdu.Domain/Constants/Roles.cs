namespace SpecEdu.Domain.Constants;

public static class Roles
{
    public const string Admin = "Admin";

    public const string SchoolAdmin = "SchoolAdmin";

    public const string Teacher = "Teacher";

    public const string Parent = "Parent";

    public const string SPP = "SPP";

    public const string PPP = "PPP";

    public const string SPC = "SPC";

    public const string Assistant = "Assistant";

    public static readonly string[] All =
    [
        Admin,
        SchoolAdmin,
        Teacher,
        Parent,
        SPP,
        PPP,
        SPC,
        Assistant
    ];
}
