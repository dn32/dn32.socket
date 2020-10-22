using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace dn32.socket
{
    public abstract class RepresentacaoDoClienteNoServidor : DnRepresentante
    {
        public override Task Conectado() => Task.CompletedTask;

        public virtual Task Conectando() => Task.CompletedTask;
    }
}
