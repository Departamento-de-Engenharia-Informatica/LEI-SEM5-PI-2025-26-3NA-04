namespace APDL.API.Domain.UserAggregate.DTO;
public class Auth0UserResponse
{
    public string user_id { get; set; }  // Necessário para gerar ticket
    public string email { get; set; }    // Se quiseres confirmar
}
