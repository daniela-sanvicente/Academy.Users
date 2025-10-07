namespace Academy.Users.Presentation.Users;

/// <summary>
/// Respuesta estándar para reportar errores sin detalles de validación por campo.
/// </summary>
public class ErrorResponseDto
{
    /// <summary>
    /// Código legible que describe el tipo de error.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje destinado al cliente para explicar el problema.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
