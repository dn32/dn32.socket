using dn32.socket.Compartilhado;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace dn32.socket
{
    internal static class Recepcao
    {
        internal static async Task ReceberInternoAsync(this DnRepresentante dnSocket)
        {
            do
            {
                try
                {
                    (ContratoDeMensagem mensagem, WebSocketReceiveResult resultado) = await dnSocket.AguardarRecebimentoAsync();
                    var retorno = await dnSocket.MensagemRecebida(mensagem.Conteudo);
                    if (mensagem.Retorno)
                    {
                        Memoria.Respostas.TryAdd(mensagem.IdDaRequisicao, mensagem); // Adiciona o retorno na lista de retornos
                    }
                    else
                    {
                       var retorno_ = new ContratoDeMensagem
                        {
                            Conteudo = retorno,
                            Retorno = true,
                            IdDaRequisicao = mensagem.IdDaRequisicao
                        };
                        await dnSocket.EnviarMensagemInternoAsync<object>(retorno_, true, mensagem.IdDaRequisicao);
                    }

                    if (resultado.CloseStatus.HasValue) break;
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException || ex is WebSocketException)
                    {
                        if (dnSocket.WebSocket.State == WebSocketState.Open)
                            try { await dnSocket.WebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "", dnSocket.Ctoken); } catch { }
                        break;
                    }

                    throw;
                }
            }
            while (!dnSocket.Ctoken.IsCancellationRequested);
            dnSocket.WebSocket?.Dispose();
        }

        private static async Task<(ContratoDeMensagem mensagem, WebSocketReceiveResult resultado)> AguardarRecebimentoAsync(this DnRepresentante dnSocket)
        {
            WebSocketReceiveResult resultado;
            using var ms = new MemoryStream();
            var buffer = new ArraySegment<byte>(new byte[8192]);
            do
            {
                resultado = await dnSocket.WebSocket.ReceiveAsync(buffer, dnSocket.Ctoken);
                if (resultado.CloseStatus.HasValue) { break; }
                ms.Write(buffer.Array, buffer.Offset, resultado.Count);
            }
            while (!resultado.EndOfMessage);
            ms.Seek(0, SeekOrigin.Begin);
            var objeto = JsonConvert.DeserializeObject<ContratoDeMensagem>(Encoding.UTF8.GetString(ms.ToArray()));
            return (objeto, resultado);
        }
    }
}