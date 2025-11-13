using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using APDL.API.Infrastructure.ShippingAgentInfrastructure;
using APDL.API.Infrastructure.Storage;
using APDL.API.Infrastructure.VesselTypes;
using APDL.API.Infrastructure.Vessels;
using APDL.API.Infrastructure.Shared;
using APDL.API.Infrastructure;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate.Repos;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.Storage;
using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.Vessels;
using APDL.API.Domain.NotificationAggregate.Repos;
using APDL.API.Infrastructure.VesselVisitInfrastructure;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Domain.ContainerAggregate.Repos;
using APDL.API.Infrastructure.ContainerInfrastructure;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ManifestAggregate.Repos;
using APDL.API.Infrastructure.ManifestInfrastructure;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.UserAggregate;
using APDL.API.Infrastructure.UserInfrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


namespace APDL.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.AddAuthentication(options =>
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
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddDbContext<DDDSample1DbContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                .ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>());

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

            app.UseRouting();

            app.UseCors("AllowAngularApp");

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureMyServices(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IShippingAgentRepository, ShippingAgentRepository>();
            services.AddScoped<ShippingAgentService>();

            services.AddScoped<IShippingAgentRepresentativeRepository, ShippingAgentRepresentativeRepository>();
            services.AddScoped<ShippingAgentRepresentativeService>();

            services.AddScoped<IFacilityRepository, FacilityRepository>();
            services.AddScoped<FacilityService>();

            services.AddScoped<IFacilityRepository, FacilityRepository>();
            services.AddScoped<FacilityService>();

            services.AddScoped<VesselTypeService>();
            services.AddScoped<IVesselTypeRepository, VesselTypeRepository>();

            services.AddScoped<VesselService>();
            services.AddScoped<IVesselRepository, VesselRepository>();

            services.AddScoped<IVesselVisitNotificationRepository, VesselVisitNotificationRepository>();
            services.AddScoped<VesselVisitNotificationService>();

            services.AddScoped<IContainerRepository, ContainerRepository>();
            services.AddScoped<ContainerService>();

            services.AddScoped<ICargoManifestRepository, CargoManifestRepository>();
            services.AddScoped<CargoManifestService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<UserService>();
        }
    }
}
