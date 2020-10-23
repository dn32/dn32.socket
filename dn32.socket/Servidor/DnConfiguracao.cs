using Microsoft.AspNetCore.Builder;

namespace dn32.socket.Servidor
{
    public static class DnConfiguracao
    {
        public static IApplicationBuilder UseDnSocket<TR>(this IApplicationBuilder app, WebSocketOptions webSocketOptions, string prefixo) where TR: DnRepresentacaoDoClienteNoServidor, new()
        {
            app = app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == $"/{prefixo}")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var cliente = new TR();
                        _= cliente.Conectando();
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        cliente.DefinirWebSocket(webSocket);
                        _= cliente.Conectado();
                        await cliente.AguardarEReceberInternoAsync();
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
