using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.socket
{
    internal static class Envio
    {
        private const int TEMPO_TE_ESPERA_POR_RETORNO_EM_MS = 20000;

        internal static async Task<To> EnviarMensagemInternoAsync<To>(this IDnRepresentante dnSocket, object mensagem, bool ehUmRetorno, Guid idDaRequisicao = default, int timeOutMs = TEMPO_TE_ESPERA_POR_RETORNO_EM_MS)
        {
            idDaRequisicao = idDaRequisicao == default ? Guid.NewGuid() : idDaRequisicao;

            using var retornoDeMensagem = new RetornoDeMensagem
            {
                IdDaRequisicao = idDaRequisicao,
                Semaforo = new SemaphoreSlim(0)
            };

            Memoria.Respostas.TryAdd(idDaRequisicao, retornoDeMensagem);

            await EnviarMensagemInternoAsync(dnSocket, mensagem, ehUmRetorno, idDaRequisicao);
            if (!ehUmRetorno)
            {
#if DEBUG
                timeOutMs = (int)TimeSpan.FromMinutes(2).TotalMilliseconds;//Todo 0h - [lembrete] Remover timeout de debug
#endif
                var sucesso = await retornoDeMensagem.Semaforo.WaitAsync(timeOutMs, dnSocket.Ctoken);
                if (!sucesso) throw new TimeoutException();
                Memoria.Respostas.TryRemove(idDaRequisicao, out var retornoDeMensagemRemover);
                retornoDeMensagemRemover?.Dispose();
                return retornoDeMensagem.Retorno == null ? default : JsonConvert.DeserializeObject<To>(retornoDeMensagem.Retorno);
            }

            return default;
        }

        private static async Task EnviarMensagemInternoAsync(this IDnRepresentante dnSocket, object mensagem, bool ehUmRetorno, Guid idDaRequisicao)
        {
            var objeto = ehUmRetorno ? mensagem : new DnContratoDeMensagem(JsonConvert.SerializeObject(mensagem), ehUmRetorno, idDaRequisicao);
            var json = JsonConvert.SerializeObject(objeto);
            var arrayDeBytes = dnSocket.UsarCompressao ? UtilZip.Zip(json) : Encoding.UTF8.GetBytes(json);

            await dnSocket.WebSocket.SendAsync(new ArraySegment<byte>(arrayDeBytes, 0, arrayDeBytes.Length), WebSocketMessageType.Binary, true, dnSocket.Ctoken);
        }

        internal static async Task EnviarMensagemAsync(this IDnRepresentante dnSocket, object mensagem)
        {
            await EnviarMensagemInternoAsync(dnSocket, mensagem, false, Guid.NewGuid());
        }
    }
}