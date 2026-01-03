using System;
using System.Linq;
using System.Security.Claims;
using APDL.API;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using APDL.API.Infrastructure;
using APDL.API.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace APDL.Tests.E2E
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DDDSample1DbContext>)
                );
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var dbName = "TestDatabase_" + Guid.NewGuid();
                services.AddDbContext<DDDSample1DbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName)
                        .ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>();
                });

                services.AddScoped<DDDSample1DbContext>(sp =>
                {
                    var options = sp.GetRequiredService<Microsoft.EntityFrameworkCore.DbContextOptions<DDDSample1DbContext>>();
                    var context = new DDDSample1DbContext(options);
                    
                    context.Database.EnsureCreated();
                    
                    if (!context.Users.Any(u => u.Email == "test@example.com"))
                    {
                        var testUser = new APDL.API.Domain.UserAggregate.User
                        {
                            Id = Guid.NewGuid(),
                            Email = "test@example.com",
                            Name = "Test User",
                            Role = "Admin",
                            IsActivated = true
                        };
                        context.Users.Add(testUser);
                        context.SaveChanges();
                    }
                    
                    return context;
                });
            });

            builder.ConfigureTestServices(services =>
            {
                var authServices = services.Where(s =>
                    s.ServiceType.FullName?.Contains("JwtBearer") == true ||
                    s.ImplementationType?.FullName?.Contains("JwtBearer") == true
                ).ToList();
                
                foreach (var service in authServices)
                {
                    services.Remove(service);
                }

                services.RemoveAll(typeof(Microsoft.AspNetCore.Authentication.AuthenticationOptions));
                
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test",
                    options => { }
                );

                services.Configure<Microsoft.AspNetCore.Authorization.AuthorizationOptions>(options =>
                {
                    options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes("Test")
                        .RequireAuthenticatedUser()
                        .Build();
                });
            });

            builder.UseEnvironment("Testing");
        }
    }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            Microsoft.Extensions.Options.IOptionsMonitor<AuthenticationSchemeOptions> options,
            Microsoft.Extensions.Logging.ILoggerFactory logger,
            System.Text.Encodings.Web.UrlEncoder encoder
        ) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim("permissions", "read:containers"),
                new Claim("permissions", "write:containers"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Role, "Port Authority Officer"),
                new Claim(ClaimTypes.Role, "Logistics Operator"),
                new Claim(ClaimTypes.Role, "Shipping Agent Representative"),
            };

            var identity = new ClaimsIdentity(claims, "Test", ClaimTypes.Name, ClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

