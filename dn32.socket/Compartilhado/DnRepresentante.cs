using dn32.socket.Interfaces;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.socket.Compartilhado
{
    public abstract class DnRepresentante : IDnRepresentante
    {
        public CancellationTokenSource CancellationTokenSource { get; }

        public bool UsarCompressao { get;  set; }

        public WebSocket WebSocket { get;  set; }

        public CancellationToken Ctoken => CancellationTokenSource.Token;

        public DnRepresentante(bool usarCompressao)
        {
            UsarCompressao = usarCompressao;
            CancellationTokenSource = new CancellationTokenSource();
        }

        public void DefinirWebSocket(WebSocket webSocket) => this.WebSocket = webSocket;

        public abstract Task<object> MensagemRecebidaAsync(string mensagem);

        public abstract Task ConectadoAsync();

        public abstract Task DesconectadoAsync(Exception exception);

        public async Task<To> EnviarMensagemComRetornoAsync<To>(object mensagem) => await this.EnviarMensagemInternoAsync<To>(mensagem, false);

        public async Task EnviarMensagemAsync(object mensagem) => await Envio.EnviarMensagemAsync(this, mensagem);

        public async Task Desconectar()
        {
            try
            {
                await WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Finalização padrão", Ctoken);
                CancellationTokenSource.Cancel(false);
                WebSocket.Dispose();
            }
            catch (WebSocketException)
            {
                //Ignore
            }
        }
    }
}