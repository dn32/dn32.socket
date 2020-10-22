using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.socket
{
    public class DnSocketServidor
    {
        async Task MaisUm(WebSocket webSocket)
        {
            {
                var buffer = new byte[1024 * 4];
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
        }
        async Task OutroAsync(WebSocket webSocket)
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var token = CancellationToken.None;
                var buffer = new ArraySegment<Byte>(new Byte[4096]);
                var received = await webSocket.ReceiveAsync(buffer, token);

                switch (received.MessageType)
                {
                    case WebSocketMessageType.Text:
                        var request = Encoding.UTF8.GetString(buffer.Array,
                                                buffer.Offset,
                                                buffer.Count);
                        var type = WebSocketMessageType.Text;
                        var data = Encoding.UTF8.GetBytes("Echo from server :" + request);
                        buffer = new ArraySegment<Byte>(data);
                        await webSocket.SendAsync(buffer, type, true, token);
                        break;
                }
            }
        }
    }
}
