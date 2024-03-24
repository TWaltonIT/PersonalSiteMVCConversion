using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using PersonalSiteMVCConversion.Data;
using PersonalSiteMVCConversion.Models;

namespace PersonalSiteMVCConversion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddOptions<CredentialSettings>().Bind(builder.Configuration.GetSection("Credentials"));
            builder.Services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = builder.Configuration.GetValue<string>("Credentials:reCAPTCHA:SiteKey"),
                SecretKey = builder.Configuration.GetValue<string>("Credentials:reCAPTCHA:SecretKey")
            });

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddOptions<CredentialSettings>().Bind(builder.Configuration.GetSection("Credentials"));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}