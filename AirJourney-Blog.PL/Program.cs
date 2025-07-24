
using AirJourney_Blog.BLL.Implementation;
using AirJourney_Blog.BLL.Interfaces;
using AirJourney_Blog.DAL.Context;
using AirJourney_Blog.DAL.Models;
using AirJourney_Blog.PL.Helper;
using AirJourney_Blog.Service.Services.Account;
using AirJourney_Blog.Service.Services.BlogCategory;
using AirJourney_Blog.Service.Services.BlogPost;
using AirJourney_Blog.Service.Services.Category;
using AirJourney_Blog.Service.Services.EmailService;
using AirJourney_Blog.Service.Services.Image;
using AirJourney_Blog.Service.Services.Resources;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;

namespace AirJourney_Blog.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string txt = "";

            // Add services to the container.

            // register data base connection
            builder.Services.AddDbContext<AppDBContext>(options =>
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("Default"),
                        sqlServerOptions =>
                        {
                            sqlServerOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        })
                    .UseLazyLoadingProxies()
                );

            builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // Lock for 15 minutes
                options.Lockout.MaxFailedAccessAttempts = 3; // Lock after 3 attempts
                options.Lockout.AllowedForNewUsers = true;

            }).AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders();


            #region Localization

            builder.Services.AddControllers()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResources));
            });


            builder.Services.AddControllersWithViews();
            builder.Services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "";
            });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                List<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-GB"),
                    new CultureInfo("es-ES")
                };

                options.DefaultRequestCulture = new RequestCulture("en-GB");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });


            #endregion



            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(options =>
            //    {
            //        options.SaveToken = true;
            //        options.RequireHttpsMetadata = false;

            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            //ValidateIssuer = true,
            //            //ValidateAudience = true,
            //            //ValidIssuer = builder.Configuration["JWT:IssuerIP"],
            //            //ValidAudience = builder.Configuration["JWT:AudienceIP"],
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(
            //                Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecurityKey"]
            //                ))
            //        };
            //    });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecurityKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });



            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(BlogCategoryProfile), typeof(BlogPostProfile), typeof(AccountProfile));
            builder.Services.AddScoped<IBlogCategoryService, BlogCategoryService>();
            builder.Services.AddScoped<IBlogPostService, BlogPostService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IImageService, ImageKitService>();
            //builder.Services.AddSingleton<ImageService>();

            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

            //builder.Services.AddControllers();
            //builder.Services.AddControllers()
            //.AddDataAnnotationsLocalization(options =>
            //{
            //    options.DataAnnotationLocalizerProvider = (type, factory) =>
            //        factory.Create(type);
            //});

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(txt,
                builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            #region Swagger Settings


            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 5 Web API",
                    Description = " ITI Projrcy"
                });
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
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


            #endregion



            #region Locatlization

            builder.Services.AddControllers();
            //.AddDataAnnotationsLocalization(options =>
            //{
            //    options.DataAnnotationLocalizerProvider = (type, factory) =>
            //        factory.Create(typeof(SharedResources));
            //});


            //builder.Services.AddControllersWithViews();
            //builder.Services.AddLocalization(opt =>
            //{
            //    opt.ResourcesPath = "";
            //});

            //builder.Services.Configure<RequestLocalizationOptions>(options =>
            //{
            //    List<CultureInfo> supportedCultures = new List<CultureInfo>
            //    {
            //        //new CultureInfo("en-US"),
            //        //new CultureInfo("de-DE"),
            //        //new CultureInfo("fr-FR"),
            //        new CultureInfo("en-GB"),
            //        new CultureInfo("es-ES")
            //    };

            //    options.DefaultRequestCulture = new RequestCulture("en-GB");
            //    options.SupportedCultures = supportedCultures;
            //    options.SupportedUICultures = supportedCultures;
            //});





            #endregion




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            #region Localization Middelware
            var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            #endregion


            app.UseHttpsRedirection();

            app.UseCors(txt);
            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
