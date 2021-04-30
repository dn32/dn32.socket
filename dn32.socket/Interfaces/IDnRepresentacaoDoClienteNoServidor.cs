
using dn32.socket.Servidor;
using System;
using System.Threading.Tasks;

namespace dn32.socket.Interfaces
{
    public interface IDnRepresentacaoDoClienteNoServidor : IDnRepresentante
    {
        DateTime UltimaRespostaDePingDoCliente { get; set; }

        Task TaskConectadoAsync { get; set; }

        void Inicializar(DnWebSocketOptions webSocketOptions);
    }
}
