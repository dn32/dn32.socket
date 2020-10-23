using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace dn32.socket
{
    public abstract class DnRepresentanteDoServidorNoCliente : DnRepresentante
    {
        internal protected ClientWebSocket ClientWebSocket => base.WebSocket as ClientWebSocket;

        public virtual Task Conectando() => Task.CompletedTask;

        public override Task Conectado() => Task.CompletedTask;

        public async Task Inicializar(string url)
        {
            await Conectando();
            var webSocket = await Conectar(url);
            DefinirWebSocket(webSocket);
            await Conectado();
            _ = this.AguardarEReceberInternoAsync();
        }

        private async Task<ClientWebSocket> Conectar(string url)
        {
            var webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri(url), CancellationTokenSource.Token);
            return webSocket;
        }
    }
}