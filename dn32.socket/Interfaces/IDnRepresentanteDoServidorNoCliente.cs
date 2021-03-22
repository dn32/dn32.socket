using dn32.socket.Interfaces;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace dn32.socket.Cliente
{
    public interface IDnRepresentanteDoServidorNoCliente : IDnRepresentante
    {
        ClientWebSocket ClientWebSocket { get; }

        Task ConectandoAsync();
        Task<ClientWebSocket> ConectarPersistenteAsync(string url, TimeSpan intervaloEntreReconexoes);
        Task Inicializar(string url, TimeSpan intervaloEntreReconexoes = default);
        Task ReconectandoAsync(Exception e, int numetoDeTentativas);
    }
}