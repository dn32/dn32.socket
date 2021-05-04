using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace dn32.socket.servidor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup("http://0.0.0.0:9009");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
