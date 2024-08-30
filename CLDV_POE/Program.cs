using CLDV_POE.Services;

namespace CLDV_POE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //register blobservice with configureation
            builder.Services.AddSingleton(new BlobService(configuration.GetConnectionString("AzureStorage")));
            //register tabel storage with configureation
            builder.Services.AddSingleton(new TableStorageService(configuration.GetConnectionString("AzureStorage")));

            // Register QueueServices woth configuration
            builder.Services.AddSingleton<QueueService>(sp =>
            {
                var connesctionString = configuration.GetConnectionString("AzureStorage");
                return new QueueService(connesctionString, "orders");
            });

            //Register fileShareServices with configureation
            builder.Services.AddSingleton<AzureFileShareService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new AzureFileShareService(connectionString, "birdshare");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
