using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.socket.Interfaces
{
    public interface IDnRepresentante : IAsyncDisposable, IDisposable
    {
        CancellationTokenSource CancellationTokenSource { get; }
        bool UsarCompressao { get; set; }
        WebSocket WebSocket { get; set; }
        CancellationToken Ctoken { get; }
        Task TaskSocketDesconectadoAsync { get; set; }
        Task TaskTratarRecepcaoERetornoAsync { get; set; }

        Task ConectadoAsync();
        void DefinirWebSocket(WebSocket webSocket);
        Task DesconectadoAsync(Exception exception);
        Task DesconectarAsync(string motivo);
        Task EnviarMensagemAsync(object mensagem);
        Task<To> EnviarMensagemComRetornoAsync<To>(object mensagem);
        Task<To> EnviarMensagemComRetornoAsync<To>(object mensagem, int timeOutMs);
        Task<object> MensagemRecebidaAsync(string mensagem);
    }
}