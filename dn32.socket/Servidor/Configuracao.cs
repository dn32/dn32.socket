using Microsoft.AspNetCore.Builder;

namespace dn32.socket.Servidor
{
    public static class Configuracao
    {
        public static IApplicationBuilder UseDnSocket<TR>(this IApplicationBuilder app, WebSocketOptions webSocketOptions, string prefixo) where TR: RepresentacaoDoClienteNoServidor, new()
        {
            app = app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == $"/{prefixo}")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var cliente = new TR();
                        await cliente.Conectando();
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        cliente.DefinirWebSocket(webSocket);
                        await cliente.Conectado();
                        await cliente.ReceberInternoAsync();
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
