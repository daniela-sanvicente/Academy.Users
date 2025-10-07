namespace Academy.Users.Presentation.Users;

/// <summary>
/// Respuesta devuelta cuando la actualización del usuario termina correctamente.
/// </summary>
public class UpdateUserSuccessResponseDto
{
    /// <summary>
    /// Identificador único del usuario actualizado.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Nombre del usuario después de la actualización.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del usuario después de la actualización.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Número telefónico normalizado del usuario.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Dirección almacenada del usuario.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Estado actual del usuario en el sistema.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje informativo sobre el resultado de la actualización.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
