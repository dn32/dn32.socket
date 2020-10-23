﻿using dn32.socket.Compartilhado;
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
        internal static async Task AguardarEReceberInternoAsync(this DnRepresentante dnSocket)
        {
            do
            {
                try
                {
                    (DnContratoDeMensagem mensagem, WebSocketReceiveResult resultado) = await dnSocket.AguardarRecebimentoAsync();
                    if (resultado.CloseStatus.HasValue)
                    {
                        _ = dnSocket.Desconectado(new InvalidOperationException(resultado.CloseStatusDescription));
                        break;
                    }

                    _ = TratarRecepcaoERetorno(dnSocket, mensagem);
                }
                catch (Exception ex)
                {
                    _ = dnSocket.Desconectado(ex);

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

        private static async Task TratarRecepcaoERetorno(DnRepresentante dnSocket, DnContratoDeMensagem mensagem)
        {
            var retorno = await dnSocket.MensagemRecebida(mensagem.Conteudo);
            if (mensagem.Retorno)
            {
                Memoria.Respostas.TryAdd(mensagem.IdDaRequisicao, mensagem);
            }
            else
            {
                var retornoEmContrato = new DnContratoDeMensagem(JsonConvert.SerializeObject(retorno), true, mensagem.IdDaRequisicao);
                await dnSocket.EnviarMensagemInternoAsync<object>(retornoEmContrato, true, mensagem.IdDaRequisicao);
            }
        }

        private static async Task<(DnContratoDeMensagem mensagem, WebSocketReceiveResult resultado)> AguardarRecebimentoAsync(this DnRepresentante dnSocket)
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
            var objeto = JsonConvert.DeserializeObject<DnContratoDeMensagem>(Encoding.UTF8.GetString(ms.ToArray()));
            return (objeto, resultado);
        }
    }
}