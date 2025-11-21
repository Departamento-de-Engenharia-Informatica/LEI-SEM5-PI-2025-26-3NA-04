using System;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ContainerAggregate.Repos;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.DockAggregate.Repos;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.ManifestAggregate.Repos;
using APDL.API.Domain.MobileEquipmentAggregate;
using APDL.API.Domain.NotificationAggregate.Repos;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.ShippingAgentAggregate.Repos;
using APDL.API.Domain.StaffQualificationAggregate;
using APDL.API.Domain.UserAggregate;
using APDL.API.Domain.Vessels;
using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Infrastructure;
using APDL.API.Infrastructure.ContainerInfrastructure;
using APDL.API.Infrastructure.DockInfrastructure;
using APDL.API.Infrastructure.ManifestInfrastructure;
using APDL.API.Infrastructure.MobileEquipmentInfrastructure;
using APDL.API.Infrastructure.Shared;
using APDL.API.Infrastructure.ShippingAgentInfrastructure;
using APDL.API.Infrastructure.StaffQualificationInfrastructure;
using APDL.API.Infrastructure.UserInfrastructure;
using APDL.API.Infrastructure.Vessels;
using APDL.API.Infrastructure.VesselTypes;
using APDL.API.Infrastructure.VesselVisitInfrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace APDL.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // services.AddDbContext<DDDSample1DbContext>(opt =>
            //     opt.UseInMemoryDatabase("DDDSample1DB")
            //     .ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>());

            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAngularApp",
                    policy =>
                    {
                        if (_env.IsDevelopment())
                        {
                            policy
                                .WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials()
                                .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                        }
                        else
                        {
                            policy
                                .WithOrigins(
                                    "http://localhost:4200",
                                    "https://10.9.11.75",
                                    "http://10.9.11.75"
                                )
                                .AllowAnyHeader() 
                                .AllowAnyMethod()
                                .AllowCredentials()
                                .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                        }
                    }
                );
            });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://apdl-operations.eu.auth0.com";
                    options.Audience = "https://localhost:5001/api";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.AddDbContext<DDDSample1DbContext>(opt =>
                opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
                    .ReplaceService<
                        IValueConverterSelector,
                        StronglyEntityIdValueConverterSelector
                    >()
            );

            ConfigureMyServices(services);

            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAngularApp");

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet(
                    "/health",
                    async context =>
                    {
                        await context.Response.WriteAsJsonAsync(
                            new
                            {
                                status = "healthy",
                                timestamp = DateTime.UtcNow,
                                version = "1.0.0",
                                environment = env.EnvironmentName,
                            }
                        );
                    }
                );
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DDDSample1DbContext>();

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Checking database...");

                    context.Database.Migrate();

                    logger.LogInformation("Database is ready!");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while creating/migrating the database");

                    // throw;
                }
            }
        }

        public void ConfigureMyServices(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IShippingAgentRepository, ShippingAgentRepository>();
            services.AddScoped<ShippingAgentService>();

            services.AddScoped<
                IShippingAgentRepresentativeRepository,
                ShippingAgentRepresentativeRepository
            >();
            services.AddScoped<ShippingAgentRepresentativeService>();

            // services.AddScoped<IFacilityRepository, FacilityRepository>();
            // services.AddScoped<FacilityService>();

            // services.AddScoped<IFacilityRepository, FacilityRepository>();
            // services.AddScoped<FacilityService>();

            services.AddScoped<VesselTypeService>();
            services.AddScoped<IVesselTypeRepository, VesselTypeRepository>();

            services.AddScoped<VesselService>();
            services.AddScoped<IVesselRepository, VesselRepository>();

            services.AddScoped<
                IVesselVisitNotificationRepository,
                VesselVisitNotificationRepository
            >();
            services.AddScoped<VesselVisitNotificationService>();

            services.AddScoped<IContainerRepository, ContainerRepository>();
            services.AddScoped<ContainerService>();

            services.AddScoped<ICargoManifestRepository, CargoManifestRepository>();
            services.AddScoped<CargoManifestService>();

            services.AddScoped<IStaffQualificationRepository, StaffQualificationRepository>();
            services.AddScoped<StaffQualificationService>();

            services.AddScoped<IDockRepository, DockRepository>();
            services.AddScoped<DockService>();

            services.AddScoped<IMobileEquipmentRepository, MobileEquipmentRepository>();
            services.AddScoped<MobileEquipmentService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<UserService>();
        }
    }
}
