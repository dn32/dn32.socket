using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.socket
{
    public abstract class DnRepresentante
    {
        protected CancellationTokenSource CancellationTokenSource { get; }

        internal protected WebSocket WebSocket { get; private set; }

        public CancellationToken Ctoken => CancellationTokenSource.Token;

        public DnRepresentante() => CancellationTokenSource = new CancellationTokenSource();

        internal void DefinirWebSocket(WebSocket webSocket) => this.WebSocket = webSocket;

        public abstract Task<object> MensagemRecebidaAsync(string mensagem);

        public abstract Task ConectadoAsync();

        public abstract Task DesconectadoAsync(Exception exception);

        public async Task<To> EnviarMensagem<To>(object mensagem) => await this.EnviarMensagemInternoAsync<To>(mensagem, false);

        public async Task Desconectar()
        {
            await WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Finalização padrão", Ctoken);
            CancellationTokenSource.Cancel(false);
            WebSocket.Dispose();
        }
    }
}