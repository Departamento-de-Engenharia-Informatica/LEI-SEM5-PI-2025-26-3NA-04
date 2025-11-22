using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.UserAggregate;
using System.Linq;
using System;
using APDL.API.Domain.UserAggregate.DTO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;



namespace APDL.API.Domain.UserAggregate
{
    public class UserService
    {
        
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;

        public UserService(IUnitOfWork unitOfWork, IUserRepository repo, IConfiguration config, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _config = config;
            _httpClient = httpClient;

        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _repo.GetByEmailAsync(email);
        }
        
        
        /*public async Task<User> CreateUserAsync(CreateUserDto dto)
        {
            var domain = _config["Auth0:Domain"];
            var token = await GetManagementApiTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 12) + "!";

            var payload = new
            {
                email = dto.Email,
                password = tempPassword,
                connection = "Username-Password-Authentication"
            };

            var createResponse = await _httpClient.PostAsJsonAsync($"{domain}/api/v2/users", payload);

            if (!createResponse.IsSuccessStatusCode)
            {
                var errorContent = await createResponse.Content.ReadAsStringAsync();
                throw new Exception($"Auth0 error: {createResponse.StatusCode} - {errorContent}");
            }

            var auth0User = await createResponse.Content.ReadFromJsonAsync<Auth0UserResponse>();

            var ticketPayload = new { user_id = auth0User.user_id };
            var ticketResponse = await _httpClient.PostAsJsonAsync($"{domain}/api/v2/tickets/password-change", ticketPayload);

            if (!ticketResponse.IsSuccessStatusCode)
            {
                var errorContent = await ticketResponse.Content.ReadAsStringAsync();
                throw new Exception($"Auth0 ticket error: {ticketResponse.StatusCode} - {errorContent}");
            }

            var ticketData = await ticketResponse.Content.ReadFromJsonAsync<Auth0PasswordChangeTicketResponse>();

            var user = new User { Email = dto.Email, Name = dto.Name, Role = dto.Role };
            await _repo.AddAsync(user);
            await _unitOfWork.CommitAsync();

            Console.WriteLine($"Password reset link: {ticketData.ticket}");

            return user;
        }*/

        
        public async Task<User> CreateUserAsync(CreateUserDto dto)
        {
            var domain = _config["Auth0:Domain"];
            var token = await GetManagementApiTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // 1. Criar utilizador no Auth0
            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 12) + "!";
            var payload = new
            {
                email = dto.Email,
                password = tempPassword,
                connection = "Username-Password-Authentication",
                email_verified = false
            };

            var createResponse = await _httpClient.PostAsJsonAsync($"{domain}/api/v2/users", payload);
            if (!createResponse.IsSuccessStatusCode)
            {
                var errorContent = await createResponse.Content.ReadAsStringAsync();
                throw new Exception($"Auth0 error: {createResponse.StatusCode} - {errorContent}");
            }

            var auth0User = await createResponse.Content.ReadFromJsonAsync<Auth0UserResponse>();

            // 2. Gerar JWT para ativação
            var activationToken = GenerateActivationToken(auth0User.user_id);

            // 3. Construir link de ativação
            var activationLink = $"{_config["App:BaseUrl"]}/auth/activate?token={activationToken}";

            // 4. Enviar email com link de ativação
            await SendActivationEmail(dto.Email, activationLink);

            // 5. Guardar utilizador na BD local
            var user = new User { Email = dto.Email, Name = dto.Name, Role = dto.Role};
            await _repo.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return user;
        }

        // Método para gerar JWT
        private string GenerateActivationToken(string userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim("purpose", "activation")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task SendActivationEmail(string toEmail, string activationLink)
        {
            var subject = "Ative a sua conta";
            var body = $@"
                <h1>Bem-vindo!</h1>
                <p>Para ativar a sua conta, clique no link abaixo:</p>
                <p><a href=""{activationLink}"">Ativar Conta</a></p>
                <p>Este link expira em 24 horas.</p>";

            using (var message = new MailMessage())
            {
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.From = new MailAddress(_config["Email:From"]);

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential(
                        _config["Email:Username"],
                        _config["Email:Password"]
                    );
                    client.EnableSsl = true;

                    await client.SendMailAsync(message);
                }
            }
        }

        
        public string ProcessActivationToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Secret"]);

            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = true
            }, out _);

            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new Exception("User ID not found in token");

            var state = Convert.ToBase64String(Encoding.UTF8.GetBytes(userId));

            var auth0LoginUrl = $"{_config["Auth0:AuthorizeUrl"]}?client_id={_config["Auth0:ClientId"]}&response_type=code&redirect_uri={_config["App:BaseUrl"]}/callback&state={state}";

            return auth0LoginUrl;
        }


        
        private async Task<string> GetManagementApiTokenAsync()
        {
            var clientId = _config["Auth0:ClientId"];
            var clientSecret = _config["Auth0:ClientSecret"];
            var domain = _config["Auth0:Domain"];

            var payload = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                audience = $"{domain}/api/v2/",
                grant_type = "client_credentials",
                scope = "create:users read:users"
            };

            var response = await _httpClient.PostAsJsonAsync($"{domain}/oauth/token", payload);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<Auth0TokenResponse>();
            return tokenResponse.access_token;
        }

    }
}