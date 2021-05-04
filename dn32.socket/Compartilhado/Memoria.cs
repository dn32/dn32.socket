using System;
using System.Collections.Concurrent;

namespace dn32.socket
{
    internal static class Memoria
    {
        public static ConcurrentDictionary<Guid, RetornoDeMensagem> Respostas { get; set; } = new ConcurrentDictionary<Guid, RetornoDeMensagem>();
    }
}
