using dn32.socket.Compartilhado;
using dn32.socket.Interfaces;
using System;
using System.Threading.Tasks;

namespace dn32.socket.Servidor
{
    public abstract class DnRepresentacaoDoClienteNoServidor : DnRepresentante, IDnRepresentacaoDoClienteNoServidor
    {
        public DateTime UltimaRespostaDePingDoCliente { get; set; }

        public override Task DesconectadoAsync(Exception exception) => Task.CompletedTask;

        public override Task ConectadoAsync() => Task.CompletedTask;

        public DnWebSocketOptions DnWebSocketOptions { get; set; }

        public DnRepresentacaoDoClienteNoServidor(bool usarCompressao) : base(usarCompressao) { }

        public void Inicializar(DnWebSocketOptions dnWebSocketOptions)
        {
            DnWebSocketOptions = dnWebSocketOptions;
            _ = EnvioConstanteDePing();
        }

        protected async Task EnvioConstanteDePing()
        {
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                if (WebSocket?.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    try
                    {
                        var retorno = await EnviarMensagemComRetornoAsync<string>("ping", (int)DnWebSocketOptions.TimeOutDePing.TotalMilliseconds);
                        if (retorno == "pong")
                        {
                            UltimaRespostaDePingDoCliente = DateTime.Now;
                            if (DnWebSocketOptions.MostrarConsoleDePing)
                            {
                                Console.WriteLine($"Cliente respondeu ao ping {UltimaRespostaDePingDoCliente:HH:mm:ss FFF}");
                            }
                        }
                        else
                        {
                            await DesconectarAsync("Timeout de dnPing");
                        }
                    }
                    catch (TimeoutException)
                    {
                        await DesconectarAsync("Timeout de dnPing");
                    }
                }

                await Task.Delay(DnWebSocketOptions.IntervaloDePing);
            }
        }
    }
}
