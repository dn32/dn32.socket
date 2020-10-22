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

        internal void DefinirWebSocket(WebSocket webSocket) => WebSocket = webSocket;

        public abstract Task<object> MensagemRecebida(string mensagem);

        public abstract Task Conectado();

        public async Task<To> EnviarMensagem<To>(object mensagem) => await this.EnviarMensagemInternoAsync<To>(mensagem, false);
    }
}