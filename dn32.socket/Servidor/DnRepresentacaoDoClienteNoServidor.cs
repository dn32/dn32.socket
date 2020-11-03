using System;
using System.Threading.Tasks;

namespace dn32.socket.servidor
{
    public abstract class DnRepresentacaoDoClienteNoServidor : DnRepresentante
    {
        public override Task DesconectadoAsync(Exception exception) => Task.CompletedTask;

        public override Task ConectadoAsync() => Task.CompletedTask;

        public DnRepresentacaoDoClienteNoServidor(bool usarCompressao) :base(usarCompressao)
        {
        }
    }
}
