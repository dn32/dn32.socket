using Polly;
using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace dn32.socket.cliente
{
    public abstract class DnRepresentanteDoServidorNoCliente : DnRepresentante
    {
        internal protected ClientWebSocket ClientWebSocket => base.WebSocket as ClientWebSocket;

        public virtual Task ConectandoAsync() => Task.CompletedTask;

        public override Task ConectadoAsync() => Task.CompletedTask;

        public virtual Task ReconectandoAsync(Exception e, int numetoDeTentativas) => Task.CompletedTask;

        public DnRepresentanteDoServidorNoCliente(bool usarCompressao) : base(usarCompressao) { }

        public async Task Inicializar(string url, TimeSpan intervaloEntreReconexoes = default)
        {
            if (intervaloEntreReconexoes == default) intervaloEntreReconexoes = TimeSpan.FromSeconds(5);

            var webSocket = await ConectarPersistenteAsync(url, intervaloEntreReconexoes);
            if (CancellationTokenSource.IsCancellationRequested) return;

            DefinirWebSocket(webSocket);
            _ = this.AguardarEReceberInternoAsync();
            _ = ConectadoAsync();
        }

        private async Task<ClientWebSocket> ConectarPersistenteAsync(string url, TimeSpan intervaloEntreReconexoes)
        {//Todo - implementar tocken de cancelamento
            var resultado = await Policy
                                .Handle<WebSocketException>()
                                .RetryForeverAsync(async (e, numetoDeTentativas) =>
                                {
                                    await Task.Delay(intervaloEntreReconexoes);
                                    _ = ReconectandoAsync(e, numetoDeTentativas);
                                })
                                .ExecuteAndCaptureAsync(async () =>
                                {
                                    if (CancellationTokenSource.IsCancellationRequested) return null;
                                    return await Conectar(url);
                                });

            return resultado?.Result;
        }

        private async Task<ClientWebSocket> Conectar(string url)
        {
            _ = ConectandoAsync();
            var webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri(url), CancellationTokenSource.Token);
            return webSocket;
        }
    }
}