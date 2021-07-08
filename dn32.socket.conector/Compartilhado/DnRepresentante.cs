using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.socket
{
    public abstract class DnRepresentante : IDnRepresentante
    {
        public CancellationTokenSource CancellationTokenSource { get; }

        public bool UsarCompressao { get; set; }

        public WebSocket WebSocket { get; set; }

        public CancellationToken Ctoken => CancellationTokenSource.Token;

        public Task TaskSocketDesconectadoAsync { get; set; }

        public Task TaskTratarRecepcaoERetornoAsync { get; set; }

        public DnRepresentante(bool usarCompressao)
        {
            UsarCompressao = usarCompressao;
            CancellationTokenSource = new CancellationTokenSource();
        }

        public void DefinirWebSocket(WebSocket webSocket) => this.WebSocket = webSocket;

        public abstract Task<object> MensagemRecebidaAsync(string mensagem);

        public abstract Task ConectadoAsync();

        public abstract void Conectado();

        public abstract Task DesconectadoAsync(Exception exception);

        public async Task<To> EnviarMensagemComRetornoAsync<To>(object mensagem) => await this.EnviarMensagemInternoAsync<To>(mensagem, false);

        public async Task<To> EnviarMensagemComRetornoAsync<To>(object mensagem, int timeOutMs) => await this.EnviarMensagemInternoAsync<To>(mensagem, false, default, timeOutMs);

        public async Task EnviarMensagemAsync(object mensagem) => await Envio.EnviarMensagemAsync(this, mensagem);

        public async Task DesconectarAsync(string motivo = "Finalização padrão")
        {
            try
            {
                await WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, motivo, Ctoken);
            }
            catch (Exception)
            { //Ignore
            }
            finally
            {
                CancellationTokenSource.Cancel(false);
                WebSocket?.Dispose();
            }
        }

        public virtual async ValueTask DisposeAsync()
        {
            Dispose();
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            CancellationTokenSource.Cancel(false);
            TaskSocketDesconectadoAsync?.Dispose();
            if (TaskTratarRecepcaoERetornoAsync.IsCompleted)
                TaskTratarRecepcaoERetornoAsync?.Dispose();
            WebSocket?.Dispose();
        }
    }
}