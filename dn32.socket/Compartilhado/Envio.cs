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

        internal static async Task<To> EnviarMensagemInternoAsync<To>(this DnRepresentante dnSocket, object mensagem, bool retorno, Guid idDaRequisicao = default)
        {
            idDaRequisicao = idDaRequisicao == default ? Guid.NewGuid() : idDaRequisicao;

            var retornoDeMensagem = new RetornoDeMensagem
            {
                IdDaRequisicao = idDaRequisicao,
                Semaforo = new SemaphoreSlim(0)
            };

            Memoria.Respostas.TryAdd(idDaRequisicao, retornoDeMensagem);

            await EnviarMensagem(dnSocket, mensagem, retorno, idDaRequisicao);
            if (!retorno)
            {
                //while (retornoDeMensagem.Retorno == null) await Task.Delay(1);
                var sucesso = await retornoDeMensagem.Semaforo.WaitAsync(TEMPO_TE_ESPERA_POR_RETORNO_EM_MS, dnSocket.Ctoken);
                if (!sucesso) throw new TimeoutException();
                Memoria.Respostas.TryRemove(idDaRequisicao, out _);
                return retornoDeMensagem.Retorno == null ? default : JsonConvert.DeserializeObject<To>(retornoDeMensagem.Retorno);
            }

            return default;
        }

        private static async Task EnviarMensagem(this DnRepresentante dnSocket, object mensagem, bool retorno, Guid idDaRequisicao)
        {
            var objeto = retorno ? mensagem : new DnContratoDeMensagem(JsonConvert.SerializeObject(mensagem), retorno, idDaRequisicao);
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objeto));
            await dnSocket.WebSocket.SendAsync(new ArraySegment<byte>(data, 0, data.Length), WebSocketMessageType.Text, true, dnSocket.Ctoken);
        }
    }
}