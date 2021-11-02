using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WSVenta.Models.Common;
using WSVenta.Services;
using WSVenta.Tools;

namespace WSVenta
{
    public class Startup
    {
        readonly string CorsConfiguration = "CorsConfiguration";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddPolicy(name: CorsConfiguration, //Politicas del Cors
                                    builder => {
                                        builder.WithHeaders("*");
                                        builder.WithOrigins("*");
                                        builder.WithMethods("*");//nos permite utilizar los metodos PUT/DELETE
                                    });
            });
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new IntToStringConverter());
                    options.JsonSerializerOptions.Converters.Add(new DecimalToStringConverter());
                });

            //todo lo qu involucra utilizar JASON WEB TOKEN
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            //a partir de aqui es JSON WEB TOKEN
            //jwt
            var appSettings = appSettingsSection.Get<AppSettings>();
            var llave = Encoding.ASCII.GetBytes(appSettings.Secreto);
            services.AddAuthentication(d =>
            {
                d.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                d.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(d => {
                    d.RequireHttpsMetadata = false;
                    d.SaveToken = true;
                    d.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(llave),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });


            //aqui vamos a inyectar. Con AddScoped mi objeto sirve por cada request.
            //por singleton existe global
            services.AddScoped<IUserService, UserService>();
            //en MVC.NET ya existen metodos para hacer inyection de dependencias
            services.AddScoped<IVentaService, VentaService>(); //ya de esta forma estamos inyectando el objeto, es decir que no es necesario crear el objeto.

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(CorsConfiguration);

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
