using CLDV_POE.Services;
using Microsoft.EntityFrameworkCore;

namespace CLDV_POE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<SqlService>(options => options.UseSqlServer(configuration.GetConnectionString("AzureSqlDatabase")));

            builder.Services.AddSingleton(new BlobService(configuration.GetConnectionString("AzureStorage")));

            builder.Services.AddSingleton(new TableStorageService(configuration.GetConnectionString("AzureStorage")));

            builder.Services.AddSingleton<QueueService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new QueueService(connectionString, "orders");
            });

            builder.Services.AddSingleton<AzureFileShareService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new AzureFileShareService(connectionString, "birdshare");
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

