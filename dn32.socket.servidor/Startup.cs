using System;
using dn32.socket.servidor.Exemplos;
using dn32.socket.Servidor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace dn32.socket.servidor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        private static WebSocketOptions WebSocketOptions => new WebSocketOptions()
        {
            KeepAliveInterval = TimeSpan.FromSeconds(120),
            ReceiveBufferSize = 4 * 1024
        };

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseDnSocket<ExemploDeRepresentacaoDeClienteNoServidor>(WebSocketOptions, "ws");

            var urlDoSocket = string.Join(", ", app.ServerFeatures.Get<IServerAddressesFeature>().Addresses).Replace("http://", "ws://") + "/ws";
         
            Console.WriteLine($"Socket aberto em: {urlDoSocket}");
        }
    }
}
