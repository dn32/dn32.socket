using System;
using System.Threading.Tasks;

namespace dn32.socket.Servidor
{
    public abstract class DnRepresentacaoDoClienteNoServidor : DnRepresentante, IDnRepresentacaoDoClienteNoServidor
    {
        public DateTime UltimaRespostaDePingDoCliente { get; set; }

        public override Task DesconectadoAsync(Exception exception) => Task.CompletedTask;

        public override Task ConectadoAsync() => Task.CompletedTask;

        public override void Conectado() { }

        public DnWebSocketOptions DnWebSocketOptions { get; set; }

        public DnRepresentacaoDoClienteNoServidor(bool usarCompressao) : base(usarCompressao) { }

        public Task TaskEnvioConstanteDePing { get; set; }

        public Task TaskConectadoAsync { get; set; }

        public void Inicializar(DnWebSocketOptions dnWebSocketOptions)
        {
            DnWebSocketOptions = dnWebSocketOptions;
            TaskEnvioConstanteDePing = EnvioConstanteDePing();
        }

        protected async Task EnvioConstanteDePing()
        {
            while (!Ctoken.IsCancellationRequested)
            {
                if (WebSocket?.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    try
                    {
                        var timeout = DnWebSocketOptions.TimeOutDePingDinamoco?.Invoke() ?? DnWebSocketOptions.TimeOutDePing;

                        var retorno = await EnviarMensagemComRetornoAsync<string>("ping", (int)timeout.TotalMilliseconds);
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

                try
                {
                    var delay = DnWebSocketOptions.IntervaloDePingDinamico?.Invoke() ?? DnWebSocketOptions.IntervaloDePing;
                    await Task.Delay(delay, Ctoken);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();

            await TaskEnvioConstanteDePing.ConfigureAwait(false);

            TaskEnvioConstanteDePing?.Dispose();
            TaskConectadoAsync?.Dispose();
        }
    }
}
