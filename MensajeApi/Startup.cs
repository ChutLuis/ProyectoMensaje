using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MensajeApi.Auth;
using MensajeApi.Mensaje;
using MensajeApi.Users;
using Metodos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MensajeApi
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
            services.AddCors();
            services.AddControllers();

            // Obtener cosas de Appsettings
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            //Configurar JWT
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            services.Configure<UsuarioDatabaseSettings>(
                Configuration.GetSection(nameof(UsuarioDatabaseSettings)));
            services.AddSingleton<IUsuarioDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<UsuarioDatabaseSettings>>().Value);
            services.Configure<ConversacionDatabaseSettings>(
                Configuration.GetSection(nameof(ConversacionDatabaseSettings)));
            services.AddSingleton<IConversacionDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<ConversacionDatabaseSettings>>().Value);
            services.Configure<MensajeDatabaseSettings>(
                Configuration.GetSection(nameof(MensajeDatabaseSettings)));
            services.AddSingleton<IMensajeDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<MensajeDatabaseSettings>>().Value);

            services.AddMvc()
            .AddSessionStateTempDataProvider();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(360);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            services.AddSingleton<Conversacion>();
            services.AddSingleton<MensajeServices>();
            services.AddSingleton<UsuarioService>();
            services.AddSingleton<ISDESRepository,SDESRepository>();
            services.AddScoped<IUsuarioService, UserService>();
            services.AddScoped<MensajeServices>();
            services.AddScoped<ConversacionServices>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(x => x
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
            app.UseSession();
            //app.UseMvcWithDefaultRoute();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
