namespace Academy.Users.Presentation.Users;

/// <summary>
/// JSON payload that contains the personal data the client wants to update.
/// </summary>
public class UpdateUserRequestDto
{
    /// <summary>
    /// Client first name. If provided, it cannot be empty and is stored as sent.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Client last name. If provided, it cannot be empty and is stored as sent.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Client phone number. Validates Mexican numbers (10 digits or +52 prefixed).
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Client address. If provided, it cannot be an empty string.
    /// </summary>
    public string? Address { get; set; }
}
