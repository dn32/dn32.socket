using dn32.socket.Compartilhado;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace dn32.socket
{
    internal static class Envio
    {
        internal static async Task<To> EnviarMensagemInternoAsync<To>(this DnRepresentante dnSocket, object mensagem, bool retorno, Guid idDaRequisicao = default)
        {
            idDaRequisicao = idDaRequisicao == default ? Guid.NewGuid() : idDaRequisicao;
            await EnviarMensagem(dnSocket, mensagem, retorno, idDaRequisicao);
            if (!retorno) return await AguardarRetorno<To>(idDaRequisicao);
            return default;
        }

        private static async Task<To> AguardarRetorno<To>(Guid idDaRequisicao)
        {
            while (true)
            {
                await Task.Delay(1); // Implementar semáforo e timeout
                if (Memoria.Respostas.TryRemove(idDaRequisicao, out var resposta))
                {
                    return resposta.Conteudo == null ? default : JsonConvert.DeserializeObject<To>(resposta.Conteudo);
                }
            }
        }

        private static async Task EnviarMensagem(this DnRepresentante dnSocket, object mensagem, bool retorno, Guid idDaRequisicao)
        {
            var objeto = retorno ? mensagem : new DnContratoDeMensagem(JsonConvert.SerializeObject(mensagem), retorno, idDaRequisicao);
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objeto));
            await dnSocket.WebSocket.SendAsync(new ArraySegment<byte>(data, 0, data.Length), WebSocketMessageType.Text, true, dnSocket.Ctoken);
        }
    }
}