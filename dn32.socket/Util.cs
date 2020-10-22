using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class Util
{
    public static ConcurrentDictionary<Guid, ContratoDeMensagem> Respostas { get; set; } = new ConcurrentDictionary<Guid, ContratoDeMensagem>();

    public static async Task Quantidade(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(3000);
            MensagemDeDebug(Respostas.Count + " Respostas esperando");
        }
    }

    public static void MensagemDeDebug(string mensagem)
    {
        Console.WriteLine(mensagem);
    }

    public static async Task EnviarMensagemDeResposta(WebSocket webSocket, CancellationToken cancellationToken, string mensagem, Guid idDaRequisicao) =>
        await EnviarMensagem(webSocket, cancellationToken, mensagem, true, idDaRequisicao);


    public static async Task<string> EnviarMensagem(WebSocket webSocket, CancellationToken cancellationToken, string mensagem)
    {
        var idDaRequisicao = Guid.NewGuid();
        await EnviarMensagem(webSocket, cancellationToken, mensagem, false, idDaRequisicao);
        while (true)
        {
            await Task.Delay(1);
            if (Respostas.TryRemove(idDaRequisicao, out var resposta))
            {
                return resposta.Conteudo; // Time-out, semáfaro etc
            }
        }
    }
    private static async Task EnviarMensagem(WebSocket webSocket, CancellationToken cancellationToken, string mensagem, bool retorno, Guid idDaRequisicao)
    {
        var objeto = new ContratoDeMensagem(mensagem, retorno, idDaRequisicao);
        var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objeto));
        await webSocket.SendAsync(new ArraySegment<byte>(data, 0, data.Length), WebSocketMessageType.Text, true, cancellationToken);
    }

    public static async Task RetornoRecebido(ContratoDeMensagem mensagem)
    {
        Respostas.TryAdd(mensagem.IdDaRequisicao, mensagem);
        await Task.CompletedTask;
    }

    public static async Task ReceberAsync(WebSocket webSocket, CancellationToken cancellationToken, Func<ContratoDeMensagem, Task<string>> recebido)
    {
        do
        {
            try
            {
                (ContratoDeMensagem mensagem, WebSocketReceiveResult resultado) = await Util.ReceberMensagem(webSocket, cancellationToken);
                var retorno = await recebido(mensagem);
                if (mensagem.Retorno)
                {
                    await RetornoRecebido(mensagem);
                }
                else
                {
                    await EnviarMensagemDeResposta(webSocket, cancellationToken, retorno, mensagem.IdDaRequisicao);
                }

                if (resultado.CloseStatus.HasValue) break;
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException || ex is WebSocketException)
                {
                    if (webSocket.State == WebSocketState.Open)
                        try { await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "", cancellationToken); } catch { }
                    break;
                }

                throw;
            }
        }
        while (!cancellationToken.IsCancellationRequested);
        webSocket?.Dispose();
    }

    private static async Task<(ContratoDeMensagem mensagem, WebSocketReceiveResult resultado)> ReceberMensagem(WebSocket webSocket, CancellationToken cancellationToken)
    {
        WebSocketReceiveResult resultado;
        using var ms = new MemoryStream();
        var buffer = new ArraySegment<byte>(new byte[8192]);
        do
        {
            resultado = await webSocket.ReceiveAsync(buffer, cancellationToken);
            if (resultado.CloseStatus.HasValue) { break; }
            ms.Write(buffer.Array, buffer.Offset, resultado.Count);
        }
        while (!resultado.EndOfMessage);
        ms.Seek(0, SeekOrigin.Begin);
        var objeto = JsonConvert.DeserializeObject<ContratoDeMensagem>(Encoding.UTF8.GetString(ms.ToArray()));
        return (objeto, resultado);
    }
}
