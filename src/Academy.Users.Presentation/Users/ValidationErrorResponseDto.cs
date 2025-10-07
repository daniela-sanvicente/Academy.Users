using System.Collections.Generic;

namespace Academy.Users.Presentation.Users;

/// <summary>
/// Respuesta utilizada cuando la solicitud contiene datos inválidos y se requiere detallar los errores.
/// </summary>
public class ValidationErrorResponseDto
{
    /// <summary>
    /// Código legible que identifica que se produjo un error de validación.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje general que resume los errores encontrados.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Conjunto de mensajes específicos para cada regla que falló.
    /// </summary>
    public IReadOnlyCollection<string> Errors { get; set; } = new List<string>();
}
