using System;
using System.Threading.Tasks;

namespace dn32.socket
{
    public abstract class DnRepresentacaoDoClienteNoServidor : DnRepresentante
    {
        public override Task Desconectado(Exception exception) => Task.CompletedTask;

        public override Task Conectado() => Task.CompletedTask;

        public virtual Task Conectando() => Task.CompletedTask;
    }
}
