namespace Academy.Users.Presentation.Users;

/// <summary>
/// Cuerpo JSON que contiene los datos personales que el cliente desea actualizar.
/// </summary>
public class UpdateUserRequestDto
{
    /// <summary>
    /// Nombre del cliente. Si se envía, no puede estar vacío y se almacena tal como se recibe.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Apellido del cliente. Si se envía, no puede estar vacío y se almacena tal como se recibe.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Número telefónico del cliente. Se validan números mexicanos de 10 dígitos o con prefijo internacional 52.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Dirección completa del cliente. Si se envía, no puede ser una cadena vacía.
    /// </summary>
    public string? Address { get; set; }
}
