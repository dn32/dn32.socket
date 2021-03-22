using dn32.socket.Compartilhado;
using dn32.socket.Interfaces;
using System;
using System.Threading.Tasks;

namespace dn32.socket.Servidor
{
    public abstract class DnRepresentacaoDoClienteNoServidor : DnRepresentante, IDnRepresentacaoDoClienteNoServidor
    {
        public override Task DesconectadoAsync(Exception exception) => Task.CompletedTask;

        public override Task ConectadoAsync() => Task.CompletedTask;

        public DnRepresentacaoDoClienteNoServidor(bool usarCompressao) :base(usarCompressao)
        {
        }
    }
}
