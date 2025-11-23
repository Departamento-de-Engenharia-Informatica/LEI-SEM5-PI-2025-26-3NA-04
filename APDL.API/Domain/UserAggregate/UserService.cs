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
using System.Text.Json;
using System.Collections.Generic;



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
        
        */
        
        
         public async Task<User> CreateUserAsync(CreateUserDto dto)
    {
        var domain = _config["Auth0:Domain"];
        var token = await GetManagementApiTokenAsync(); 

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);


        var existingAuth0User = await GetAuth0UserByEmailAsync(dto.Email);
        
        if (existingAuth0User != null)
        {
             throw new Exception("O utilizador com este email já se encontra registado.");
        }
        
        Auth0UserResponse auth0User = existingAuth0User;

        if (auth0User == null)
        {
            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 12) + "!";
            
            var payload = new
            {
                email = dto.Email,
                password = tempPassword,
                connection = "Username-Password-Authentication"
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var createResponse = await _httpClient.PostAsync($"{domain}/api/v2/users", content);

            if (!createResponse.IsSuccessStatusCode)
            {
                var errorContent = await createResponse.Content.ReadAsStringAsync();
                throw new Exception($"Auth0 creation error: {createResponse.StatusCode} - {errorContent}");
            }

            var auth0ResponseContent = await createResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Auth0 Creation Response JSON: {auth0ResponseContent}"); 

            try
            {
                auth0User = JsonSerializer.Deserialize<Auth0UserResponse>(auth0ResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to deserialize Auth0 user response. Raw content: {auth0ResponseContent}. Error: {ex.Message}");
            }

            if (auth0User == null)
            {
                throw new Exception($"Auth0 user was created successfully but returned null/empty data. Raw content: {auth0ResponseContent}");
            }
        }
        
        var redirectUrlWithUser = $"{_config["App:BaseUrl"]}/callback?userId={auth0User.user_id}";
        var ticketPayload = new
        {
            user_id = auth0User.user_id,
            includeEmailInRedirect = true, 
            result_url = redirectUrlWithUser 
        };

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

        await SendActivationEmail(dto.Email, ticketData.ticket);

        Console.WriteLine($"Password reset link: {ticketData.ticket}");

        return user;
    }

    private async Task<Auth0UserResponse> GetAuth0UserByEmailAsync(string email)
    {
        var domain = _config["Auth0:Domain"];
        var encodedEmail = Uri.EscapeDataString($"email:\"{email}\"");
        var queryUrl = $"{domain}/api/v2/users?q={encodedEmail}&search_engine=v3";


        var response = await _httpClient.GetAsync(queryUrl);

        if (response.IsSuccessStatusCode)
        {
            var users = await response.Content.ReadFromJsonAsync<List<Auth0UserResponse>>();
            return users.FirstOrDefault();
        }

        return null;
    }

    private async Task<string> GetManagementApiTokenAsync()
    {
        using var tempClient = new HttpClient();
        
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

        var response = await tempClient.PostAsJsonAsync($"{domain}/oauth/token", payload);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<Auth0TokenResponse>();
        return tokenResponse.access_token;
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

    }
}