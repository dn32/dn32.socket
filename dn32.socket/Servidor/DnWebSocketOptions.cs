using Microsoft.AspNetCore.Builder;
using System;

namespace dn32.socket.Servidor
{
    public class DnWebSocketOptions : WebSocketOptions
    {
        public TimeSpan IntervaloDePing { get; set; }
        public TimeSpan TimeOutDePing { get; set; }
        public bool MostrarConsoleDePing { get; set; }
    }
}
