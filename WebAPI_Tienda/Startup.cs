﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

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
            // Agrega Automapper
            services.AddAutoMapper(typeof(Startup));
            // Abre Base de datos
            services.AddDbContext<TiendaContext>(
                options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );
            services.AddDatabaseDeveloperPageExceptionFilter();
            // Agrega MVC
            services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
            );
            // Agrega Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(
                c =>
                {
                    // Agrega autorización a swagger
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI_Tienda", Version = "v1" });
                    // Agrega autorización a Swagger
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new String[]{}
                        }
                    });
                }
            );
            // Agrega automapper
            services.AddAutoMapper(typeof(Startup));
            // Agrega autorización e identidad
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                opciones =>
                    opciones.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Keyjwt"])),
                        ClockSkew = TimeSpan.Zero
                    }
                );

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<TiendaContext>()
                .AddDefaultTokenProviders();

            // Agrega políticas de autorización
            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("RequiereAdmin", politica => politica.RequireClaim("esAdmin"));
            });

            //Agrega CORS (Cross Origin Resource Sharing)
            services.AddCors(opciones =>
            {
                opciones.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("https://apirequest.io").AllowAnyMethod().AllowAnyHeader();
                    //builder.WithOrigins("https://google.com").AllowAnyMethod().AllowAnyHeader();
                });
            });
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
