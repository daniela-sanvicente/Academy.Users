using System.Collections.Generic;

namespace Academy.Users.Presentation.Users;

/// <summary>
/// Response used when the request contains invalid data and field errors need to be listed.
/// </summary>
public class ValidationErrorResponseDto
{
    /// <summary>
    /// Machine-readable status indicating a validation failure.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// General message summarizing the validation issues.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Collection of specific error messages per failed rule.
    /// </summary>
    public IReadOnlyCollection<string> Errors { get; set; } = new List<string>();
}
