using System;
using System.Diagnostics;
using System.Threading.Tasks;
using dn32.socket.servidor.Exemplos;
using dn32.socket.Servidor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace dn32.socket.servidor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddTransient<IExemploDeRepresentacaoDeClienteNoServidor,  ExemploDeRepresentacaoDeClienteNoServidor>();
        }

        private static DnWebSocketOptions DnWebSocketOptions => new()
        {
            IntervaloDePing = TimeSpan.FromSeconds(10),
            TimeOutDePing = TimeSpan.FromSeconds(10),
            MostrarConsoleDePing = true,
        };

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("API está aqui");
                });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseDnSocket<IExemploDeRepresentacaoDeClienteNoServidor>(DnWebSocketOptions, "ws");

            var urlDoSocket = string.Join(", ", app.ServerFeatures.Get<IServerAddressesFeature>().Addresses).Replace("http://", "ws://") + "/ws";

            Console.WriteLine($"Socket aberto em: {urlDoSocket}");
            int contador = 0;

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    contador++;
                    if (contador % 10 == 0)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        Console.WriteLine($"Limpar");
                    }

                    var myProcess = Process.GetCurrentProcess();
                    Console.WriteLine($"Con: {DnConfiguracao.Conectados} - Memoria: " + (int)(myProcess.PrivateMemorySize64 / 1024 / 1024) + "MB");
                }
            });
        }
    }
}
