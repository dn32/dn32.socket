using System;
using System.Collections.Concurrent;

namespace dn32.socket.Compartilhado
{
    internal static class Memoria
    {
        public static ConcurrentDictionary<Guid, ContratoDeMensagem> Respostas { get; set; } = new ConcurrentDictionary<Guid, ContratoDeMensagem>();
    }
}
