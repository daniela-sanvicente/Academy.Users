namespace Academy.Users.Presentation.Users;

/// <summary>
/// Response returned when the user update succeeds.
/// </summary>
public class UpdateUserSuccessResponseDto
{
    /// <summary>
    /// Unique identifier of the updated user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// User first name after the update.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User last name after the update.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User phone number after normalization.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// User address as stored after the update.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// User status in the system.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Informational message about the update result.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
