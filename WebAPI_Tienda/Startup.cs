using Microsoft.EntityFrameworkCore;

namespace WebAPI_Tienda
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        // Inyecta servicios al contenedor
        public void ConfigureServices(IServiceCollection services)
        {
            // Abre Base de datos
            services.AddDbContext<TiendaContext>(
                options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );
            services.AddDatabaseDeveloperPageExceptionFilter();
            // Agrega MVC
            services.AddControllers();
            // Agrega Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        // Middleware
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
