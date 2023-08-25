using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Identity.Models;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity;
public class Startup
{
    public IConfiguration configRoot
    {
        get;
    }
    readonly string MyAllowSpecificOrigins = "CorsPolicy";

    public Startup(IConfiguration configuration)
    {
        configRoot = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationContext>(opts =>
            opts.UseSqlServer(configRoot.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddCookie()
            .AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateActor = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configRoot["Issuer"],
                    ValidAudience = configRoot["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                        (configRoot["SigningKey"]))
                };
            });


        services.Configure<AppSettings>(configRoot);

        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 3;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 0;
            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 1000;
            options.Lockout.AllowedForNewUsers = true;
            // User settings
            options.User.RequireUniqueEmail = false;
            options.SignIn.RequireConfirmedEmail = false;
        });

        services.AddAutoMapper(typeof(Startup));
        services.AddEndpointsApiExplorer();
        //services.AddSwaggerGen();

        //services.AddSwaggerGen(swagger =>
        //{
        //    //This is to generate the Default UI of Swagger Documentation
        //    swagger.SwaggerDoc("v1", new OpenApiInfo
        //    {
        //        Version = "v1",
        //        Title = " Identity api",
        //        Description = "ASP.NET Core 7.0 Web API"
        //    });
        //    // To Enable authorization using Swagger (JWT)

        //});


        services.AddSwaggerGen(swagger =>
        {
            //This is to generate the Default UI of Swagger Documentation
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = " Identity api",
                Description = "ASP.NET Core 7.0 Web API"
            });
            // To Enable authorization using Swagger (JWT)
            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            });
            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] {}

                }
            });

        });




        services.AddControllers();
         services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

    }
    public void Configure(WebApplication app, IWebHostEnvironment env)
    {

        app.UseSwagger();
        
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        });
        app.UseRouting();
        app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            //endpoints.MapControllerRoute("default", "api/{controller=Home}/{action=Index}/{id?}");

        });
        app.Run();
    }
}










