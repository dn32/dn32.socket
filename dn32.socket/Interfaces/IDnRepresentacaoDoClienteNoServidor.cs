
using System;

namespace dn32.socket.Interfaces
{
    public interface IDnRepresentacaoDoClienteNoServidor : IDnRepresentante
    {
        DateTime UltimaRespostaDePingDoCliente { get; set; }
    }
}
