using System;
using dn32.socket.servidor.Exemplos;
using dn32.socket.Servidor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
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

        private static WebSocketOptions WebSocketOptions => new()
        {
            KeepAliveInterval = TimeSpan.FromSeconds(12)
        };

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseDnSocket<IExemploDeRepresentacaoDeClienteNoServidor>(WebSocketOptions, "ws");

            var urlDoSocket = string.Join(", ", app.ServerFeatures.Get<IServerAddressesFeature>().Addresses).Replace("http://", "ws://") + "/ws";
         
            Console.WriteLine($"Socket aberto em: {urlDoSocket}");
        }
    }
}
