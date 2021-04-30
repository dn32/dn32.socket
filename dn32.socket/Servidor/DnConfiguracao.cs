using dn32.socket.Compartilhado;
using dn32.socket.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.socket.Servidor
{
    public static class DnConfiguracao
    {
        private static Pipe _pipe;
        public static int Total = 0;
        public static int Conectados = 0;
        public static int Eliminar = 0;
        public static void Limpar(int quantidade)
        {
            Eliminar = quantidade;
        }

        public static IApplicationBuilder UseDnSocket<TR>(this IApplicationBuilder app, DnWebSocketOptions webSocketOptions, string prefixo) where TR : IDnRepresentacaoDoClienteNoServidor
        {
            app = app.UseWebSockets(webSocketOptions);
            var canc = new CancellationTokenSource();
            _pipe = new Pipe();


            app.Use(async (context, next) =>
            {
                if (context.Request.Path == $"/{prefixo}")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using (var scope = app.ApplicationServices.CreateScope())
                        {
                            var cliente = scope.ServiceProvider.GetRequiredService<TR>();
                            if (cliente == null)
                                throw new InvalidOperationException($"{typeof(TR).Name} não foi encontrado na injeção de dependência. Registro-o da sequinte forma: services.AddTransient<MeuServico>();");

                            cliente.Inicializar(webSocketOptions);

                            Interlocked.Increment(ref Conectados);
                            Interlocked.Increment(ref Total);

                            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            cliente.DefinirWebSocket(webSocket);
                            cliente.TaskConectadoAsync = cliente.ConectadoAsync();
                            await cliente.AguardarEReceberInternoAsync(cliente);
                            //    try
                            //    {
                            //        await AguardarRecebimentoAsync2(webSocket);
                            //}
                            //catch { }

                            //cliente.TaskSocketDesconectadoAsync = cliente.DesconectadoAsync(null);
                            //#if NETSTANDARD2_1
                            //#else
                            //try
                            //{
                            //    await webSocket.CloseOutputAsync(WebSocketCloseStatus.EndpointUnavailable, "Request finalizada", new CancellationToken());
                            //}
                            //catch (Exception) { }
                            //#endif
                        }

                        Interlocked.Decrement(ref Conectados);
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

        private const int BUFFER_SIZE = 8192;

        private static async Task AguardarRecebimentoAsync2(WebSocket dnSocket)
        {
            while (true)
            {
                WebSocketReceiveResult resultado;
                using var ms = new MemoryStream();
                var buffer = new ArraySegment<byte>(new byte[8192]);
                do
                {
                    resultado = await dnSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (resultado.CloseStatus.HasValue) { break; }
                    ms.Write(buffer.Array, buffer.Offset, resultado.Count);
                }
                while (!resultado.EndOfMessage);
                ms.Seek(0, SeekOrigin.Begin);

                //var arrayDeBytes = ms.ToArray();
                //var json = dnSocket.UsarCompressao ? UtilZip.Unzip(arrayDeBytes) : Encoding.UTF8.GetString(arrayDeBytes);

                //var objeto = JsonConvert.DeserializeObject<DnContratoDeMensagem>(json);
                //return (objeto, resultado);
            }
        }

        private static async Task AguardarRecebimentoAsync1(WebSocket WebSocket)
        {
            while (true)
            {
                var writer = _pipe.Writer;

                do
                {
                    var memory = writer.GetMemory(BUFFER_SIZE);
                    var receiveResult = await WebSocket.ReceiveAsync(memory, CancellationToken.None);
                    if (!receiveResult.EndOfMessage)
                    {
                        writer.Advance(receiveResult.Count);
                        continue;
                    }

                    await writer.FlushAsync(CancellationToken.None);

                    //await guildPlayer.OnMessageAsync(_pipe.Reader);
                } while (WebSocket.State == WebSocketState.Open);
                //}
                //catch (Exception exception)
                //{
                //    //await writer.CompleteAsync(exception);
                //    //await guildPlayer.OnDisconnectedAsync(exception);
                //}
            }
        }
    }
}
