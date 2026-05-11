using gdm5._0.Data;
using gdm5._0.Models;
using gdm5._0.Services;
using gdm5._0.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace gdm5._0
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
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string conString = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(conString));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddHttpContextAccessor();
            services.AddSingleton<IUriService>(options =>
            {
                var accessor = options.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });

            //services.AddDefaultIdentity<ApplicationUser>(options => 
            //          options.SignIn.RequireConfirmedAccount = true)
            //         .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            // Identity Server removed - using JWT Bearer authentication only (configured below)



            //.AddCookie(config =>
            // {
            //     config.Cookie.Name = "auth";
            //     config.Cookie.HttpOnly = true;//The cookie cannot be obtained by the front-end or the browser, and can only be modified on the server side
            //                                   // config.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;//This cookie cannot be used as a third-party cookie under any circumstances, without exception. For example, suppose b.com sets the following cookies:
            // })


            var jwtSettings = Configuration.GetSection("JwtSettings");

            services.AddAuthentication(options =>
           {
               options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
             
               //options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
               //}).AddCookie("CookiewAuth", options => {
               //    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("Identity/Account/Login");


           }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = false,
                    ClockSkew = TimeSpan.Zero,
                    //ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                    //ValidAudience = jwtSettings.GetSection("validAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("securityKey").Value))

                };

            });
            // .AddIdentityServerJwt() removed - not needed for JWT Bearer authentication

            services.AddControllersWithViews();

            services.AddRazorPages();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist/gdm5._0";
             //   configuration.RootPath = "ClientApp/dist";
            });

            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IRegistrationService, RegistrationService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IMetadataService, MetadataService>();
            services.AddTransient<IProductTypeService, ProductTypeService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IWareHouseService, WareHouseService>();
            services.AddTransient<ICurrencyService, CurrencyService>();
            services.AddTransient<IComplexPriceListService, ComplexPriceListService>();
            services.AddTransient<IPriceListService, PriceListService>();
            services.AddTransient<IPriceListValueService, PriceListValueService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseCors("EnableCORS");

            app.UseHttpsRedirection();
            // app.UseIdentityServer(); // Removed - not needed with JWT Bearer
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
         
            //var ci = new CultureInfo("en-US");
            //ci.DateTimeFormat.LongDatePattern = "MM/dd/yyyy";
            //app.UseRequestLocalization(new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(ci),
            //    SupportedCultures = new List<CultureInfo> { ci },
            //    SupportedUICultures = new List<CultureInfo> { ci }
            //});
            

            app.UseAuthentication();
            // app.UseIdentityServer();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501
                //  spa.Options.SourcePath = "ClientApp";
                spa.Options.SourcePath = "ClientApp/dist";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }

                //   spa.UseAngularCliServer(npmScript: "start");
            });
            //app.UseSpa(spa =>
            //{
            //    // To learn more about options for serving an Angular SPA from ASP.NET Core,
            //    // see https://go.microsoft.com/fwlink/?linkid=864501

            //    spa.Options.SourcePath = "ClientApp";

            //    if (env.IsDevelopment())
            //    {
            //        spa.UseAngularCliServer(npmScript: "start");
            //    }

            //   // spa.UseAngularCliServer(npmScript: "start");
            //});
        }
    }
}
