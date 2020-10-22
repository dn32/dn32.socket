using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace dn32.socket
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

            app = app.UseWebSockets(WebSocketOptions);

            var CancellationTokenSource = new CancellationTokenSource();

            _ = Util.Quantidade(CancellationTokenSource.Token);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Util.ReceberAsync(webSocket, CancellationTokenSource.Token,
                            async (ContratoDeMensagem mensagem) =>
                        {
                            Util.MensagemDeDebug(mensagem.Conteudo);
                            await Task.CompletedTask;
                            return "Olá teste é o caralho, meu nome é Zé Piqueno!";
                        });
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
        }
    }
}
