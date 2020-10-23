using System;
using System.Collections.Concurrent;

namespace dn32.socket.Compartilhado
{
    internal static class Memoria
    {
        public static ConcurrentDictionary<Guid, DnContratoDeMensagem> Respostas { get; set; } = new ConcurrentDictionary<Guid, DnContratoDeMensagem>();
    }
}
