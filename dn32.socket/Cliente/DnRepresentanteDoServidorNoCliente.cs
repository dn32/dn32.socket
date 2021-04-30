using dn32.socket.Compartilhado;
using dn32.socket.Interfaces;
using Polly;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace dn32.socket.Cliente
{
    public abstract class DnRepresentanteDoServidorNoCliente : DnRepresentante, IDnRepresentanteDoServidorNoCliente
    {
        public ClientWebSocket ClientWebSocket => base.WebSocket as ClientWebSocket;

        public Task TaskAguardarEReceberInternoAsync { get; private set; }
        public Task TaskConectadoAsync { get; private set; }
        public Task TaskReconectandoAsync { get; private set; }
        public Task TaskConectandoAsync { get; private set; }

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
            TaskAguardarEReceberInternoAsync = this.AguardarEReceberInternoAsync(this);
            TaskConectadoAsync = ConectadoAsync();
        }

        public async Task<ClientWebSocket> ConectarPersistenteAsync(string url, TimeSpan intervaloEntreReconexoes)
        {
            var resultado = await Policy
                                .Handle<WebSocketException>()
                                .RetryForeverAsync(async (e, numetoDeTentativas) =>
                                {
                                    await Task.Delay(intervaloEntreReconexoes);
                                    TaskReconectandoAsync = ReconectandoAsync(e, numetoDeTentativas);
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
            TaskConectandoAsync = ConectandoAsync();
            var webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri(url), CancellationTokenSource.Token);
            return webSocket;
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            TaskAguardarEReceberInternoAsync?.Dispose();
            TaskConectadoAsync?.Dispose();
            TaskReconectandoAsync?.Dispose();
            TaskConectandoAsync?.Dispose();
        }
    }
}