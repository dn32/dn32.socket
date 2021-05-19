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
        internal static async Task AguardarEReceberInternoAsync<TR>(this IDnRepresentante dnSocket, TR cliente) where TR : IDnRepresentante
        {
            do
            {
                try
                {
                    (DnContratoDeMensagem mensagem, WebSocketReceiveResult resultado) = await dnSocket.AguardarRecebimentoAsync();
                    if (resultado.CloseStatus.HasValue)
                    {
                        cliente.TaskSocketDesconectadoAsync = dnSocket.DesconectadoAsync(new InvalidOperationException(resultado.CloseStatusDescription));
                        break;
                    }

                    cliente.TaskTratarRecepcaoERetornoAsync = TratarRecepcaoERetornoAsync(dnSocket, mensagem);
                }
                catch (Exception ex)
                {
                    cliente.TaskSocketDesconectadoAsync = dnSocket.DesconectadoAsync(ex);

                    if (ex is OperationCanceledException || ex is WebSocketException)
                    {
                        if (dnSocket.WebSocket.State == WebSocketState.Open)
                            try { await dnSocket.WebSocket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, "", dnSocket.Ctoken); } catch { }
                        break;
                    }

                    throw;
                }
            }
            while (!dnSocket.Ctoken.IsCancellationRequested);
            dnSocket.WebSocket?.Dispose();
        }

        private static async Task TratarRecepcaoERetornoAsync(IDnRepresentante dnSocket, DnContratoDeMensagem mensagem)
        {
            if (mensagem.Retorno)
            {
                if (Memoria.Respostas.TryGetValue(mensagem.IdDaRequisicao, out var retornoDeMensagem))
                {
                    retornoDeMensagem.Retorno = mensagem.Conteudo;
                    if (retornoDeMensagem.Disposed == false)
                        retornoDeMensagem.Semaforo.Release();
                }
            }
            else
            {
                DnContratoDeMensagem retornoEmContrato;

                if (mensagem.Conteudo == "\"ping\"")
                {
                    retornoEmContrato = new DnContratoDeMensagem("\"pong\"", true, mensagem.IdDaRequisicao);
                }
                else
                {
                    var objetoDeRetorno = await dnSocket.MensagemRecebidaAsync(mensagem.Conteudo);
                    retornoEmContrato = new DnContratoDeMensagem(JsonConvert.SerializeObject(objetoDeRetorno), true, mensagem.IdDaRequisicao);
                }

                await dnSocket.EnviarMensagemInternoAsync<object>(retornoEmContrato, true, mensagem.IdDaRequisicao);
            }
        }

        private static async Task<(DnContratoDeMensagem mensagem, WebSocketReceiveResult resultado)> AguardarRecebimentoAsync(this IDnRepresentante dnSocket)
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

            var arrayDeBytes = ms.ToArray();
            var json = dnSocket.UsarCompressao ? UtilZip.Unzip(arrayDeBytes) : Encoding.UTF8.GetString(arrayDeBytes);

            var objeto = JsonConvert.DeserializeObject<DnContratoDeMensagem>(json);
            return (objeto, resultado);
        }
    }
}