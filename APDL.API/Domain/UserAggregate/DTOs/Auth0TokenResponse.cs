namespace APDL.API.Domain.UserAggregate.DTO;
public class Auth0TokenResponse
{
    public string id_token { get; set; } 
    public string access_token { get; set; } 
    public string token_type { get; set; } 
    public int expires_in { get; set; }
}
