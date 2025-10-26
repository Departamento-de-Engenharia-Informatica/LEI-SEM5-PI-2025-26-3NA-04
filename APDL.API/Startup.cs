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

            app.UseSwagger();
            app.UseSwaggerUI();

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

            services.AddScoped<IVesselTypeRepository, VesselTypeRepository>();
            services.AddScoped<VesselTypeService>();

            services.AddTransient<IFacilityRepository, FacilityRepository>();
            services.AddTransient<FacilityService>();

            services.AddTransient<VesselTypeService>();
            services.AddTransient<IVesselTypeRepository, VesselTypeRepository>();

            services.AddTransient<VesselService>();
            services.AddTransient<IVesselRepository, VesselRepository>();

        }
    }
}
