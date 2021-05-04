using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Pipelines;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.socket.Servidor
{
    public static class DnConfiguracao
    {
        public static long TotalDeConexoesJaRealizadas = 0;
        public static int SocketsConectados = 0;

        public static IApplicationBuilder UseDnSocket<TR>(this IApplicationBuilder app, DnWebSocketOptions webSocketOptions, string prefixo) where TR : IDnRepresentacaoDoClienteNoServidor
        {
            app = app.UseWebSockets(webSocketOptions);
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == $"/{prefixo}")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        try
                        {
                            Interlocked.Increment(ref SocketsConectados);
                            Interlocked.Increment(ref TotalDeConexoesJaRealizadas);

                            await IniciarUmaConexao<TR>(app, webSocketOptions, context);
                        }
                        catch (Exception ex)
                        {
                            await TratarExcecao(context, ex);
                        }
                        finally
                        {
                            Interlocked.Decrement(ref SocketsConectados);
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

        private static async Task TratarExcecao(HttpContext context, Exception ex)
        {
            if (!context.Response.Headers.IsReadOnly)
            {
                context.Response.StatusCode = context.Response.StatusCode == (int)HttpStatusCode.OK ? (int)HttpStatusCode.InternalServerError : context.Response.StatusCode;
                if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin")) context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Headers")) context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
                if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Methods")) context.Response.Headers.Add("Access-Control-Allow-Methods", "*");
            }

            await context.Response.WriteAsync(ex.Message);
        }

        private static async Task IniciarUmaConexao<TR>(IApplicationBuilder app, DnWebSocketOptions webSocketOptions, HttpContext context) where TR : IDnRepresentacaoDoClienteNoServidor
        {
            using var scope = app.ApplicationServices.CreateScope();
            var cliente = scope.ServiceProvider.GetRequiredService<TR>();
            if (cliente == null)
                throw new InvalidOperationException($"{typeof(TR).Name} não foi encontrado na injeção de dependência. Registro-o da sequinte forma: services.AddTransient<MeuServico>();");

            cliente.Inicializar(webSocketOptions);

            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            cliente.DefinirWebSocket(webSocket);
            cliente.TaskConectadoAsync = cliente.ConectadoAsync();
            cliente.Conectado();
            await cliente.AguardarEReceberInternoAsync(cliente);

            if (webSocket.State == WebSocketState.Open)
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.EndpointUnavailable, "Request finalizada", new CancellationToken());
        }
    }
}
