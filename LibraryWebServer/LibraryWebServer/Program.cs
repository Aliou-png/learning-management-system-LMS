using LibraryWebServer.Models;
using Microsoft.EntityFrameworkCore;
using LibraryWebServer.Models;

namespace LibraryWebServer
{
  public class Program
  {
    public static void Main( string[] args )
    {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.
      builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<Team3LibraryContext>(options =>
         options.UseMySql(
            "server=atr.eng.utah.edu;user id=u1415075;password=CS_5530_2026;database=Team3Library",
            Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.16-mariadb")
         ));
            var app = builder.Build();

      // Configure the HTTP request pipeline.
      if ( !app.Environment.IsDevelopment() )
      {
        app.UseExceptionHandler( "/Home/Error" );
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}" );

      app.Run();
    }
  }
}