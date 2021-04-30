using dn32.socket.Compartilhado;
using dn32.socket.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace dn32.socket.Servidor
{
    public static class DnConfiguracao
    {
        public static IApplicationBuilder UseDnSocket<TR>(this IApplicationBuilder app, DnWebSocketOptions webSocketOptions, string prefixo, Func<ServiceProvider> registrar, IServiceScopeFactory serviceScopeFactory) where TR : IDnRepresentacaoDoClienteNoServidor
        {
            app = app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == $"/{prefixo}")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using (var scope = app.ApplicationServices.CreateScope())
                        {
                            var cliente = scope.ServiceProvider.GetRequiredService<TR>();
                            cliente.Inicializar(webSocketOptions);
                            if (cliente == null)
                                throw new InvalidOperationException($"{typeof(TR).Name} não foi encontrado na injeção de dependência. Registro-o da sequinte forma: services.AddTransient<MeuServico>();");

                            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            cliente.DefinirWebSocket(webSocket);
                            cliente.TaskConectadoAsync = cliente.ConectadoAsync();
                            await cliente.AguardarEReceberInternoAsync(cliente);
                            cliente.TaskSocketDesconectadoAsync = cliente.DesconectadoAsync(null);
                        }
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

            return app;
        }
    }
}
