using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.socket.Interfaces
{
    public interface IDnRepresentante
    {
        CancellationTokenSource CancellationTokenSource { get; }
        bool UsarCompressao { get; set; }
        WebSocket WebSocket { get; set; }
        CancellationToken Ctoken { get; }
        Task ConectadoAsync();
        void DefinirWebSocket(WebSocket webSocket);
        Task DesconectadoAsync(Exception exception);
        Task DesconectarAsync(string motivo);
        Task EnviarMensagemAsync(object mensagem);
        Task<To> EnviarMensagemComRetornoAsync<To>(object mensagem);
        Task<object> MensagemRecebidaAsync(string mensagem);
    }
}