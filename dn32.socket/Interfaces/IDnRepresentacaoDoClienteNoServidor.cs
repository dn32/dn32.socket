
using dn32.socket.Servidor;
using System;

namespace dn32.socket.Interfaces
{
    public interface IDnRepresentacaoDoClienteNoServidor : IDnRepresentante
    {
        DateTime UltimaRespostaDePingDoCliente { get; set; }

        void Inicializar(DnWebSocketOptions webSocketOptions);
    }
}
