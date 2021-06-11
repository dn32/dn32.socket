using Microsoft.AspNetCore.Builder;
using System;

namespace dn32.socket.Servidor
{
    public class DnWebSocketOptions : WebSocketOptions
    {
        public TimeSpan IntervaloDePing { get; set; } = TimeSpan.FromSeconds(120);
        public TimeSpan TimeOutDePing { get; set; } = TimeSpan.FromSeconds(120);
        public bool MostrarConsoleDePing { get; set; }

        public Func<TimeSpan> IntervaloDePingDinamico { get; set; }
        public Func<TimeSpan> TimeOutDePingDinamoco { get; set; }
    }
}
