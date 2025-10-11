namespace Academy.Users.Presentation.Users;

/// <summary>
/// Standard error payload without per-field validation details.
/// </summary>
public class ErrorResponseDto
{
    /// <summary>
    /// Machine-readable status that identifies the error category.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable message returned to the client.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
