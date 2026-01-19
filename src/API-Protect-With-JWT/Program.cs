using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace API_Protect_With_JWT;

public static class Program
{
    public static void Main(string[] args) =>
        CreateWebHostBuilder(args).Build().Run();

    public static IHostBuilder CreateWebHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}