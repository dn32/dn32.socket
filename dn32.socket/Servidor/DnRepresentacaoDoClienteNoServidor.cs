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

        public DnRepresentacaoDoClienteNoServidor(bool usarCompressao, int tempoDeEsperaParaEnvioDePingMs, bool mostrarPingPongNoConsole = false) : base(usarCompressao)
        {
            _ = EnvioConstanteDePing(tempoDeEsperaParaEnvioDePingMs, mostrarPingPongNoConsole);
        }

        protected async Task EnvioConstanteDePing(int tempoDeEsperaParaEnvioDePingMs, bool mostrarPingPongNoConsole = false)
        {
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                if (WebSocket?.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    var retorno = await EnviarMensagemComRetornoAsync<string>("ping", tempoDeEsperaParaEnvioDePingMs);
                    if (retorno == "pong")
                    {
                        UltimaRespostaDePingDoCliente = DateTime.Now;
                        if (mostrarPingPongNoConsole)
                        {
                            Console.WriteLine($"Cliente respondeu ao ping {UltimaRespostaDePingDoCliente}");
                        }
                    }
                    else
                    {
                        await DesconectarAsync("Timeout de dnPing");
                    }
                }

                await Task.Delay(tempoDeEsperaParaEnvioDePingMs);
            }
        }
    }
}
