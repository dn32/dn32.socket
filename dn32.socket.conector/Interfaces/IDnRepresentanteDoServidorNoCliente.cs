using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace dn32.socket
{
    public interface IDnRepresentanteDoServidorNoCliente : IDnRepresentante
    {
        ClientWebSocket ClientWebSocket { get; }

        Task ConectandoAsync();
        Task<ClientWebSocket> ConectarPersistenteAsync(string url, TimeSpan intervaloEntreReconexoes);
        Task InicializarAsync(string url, TimeSpan intervaloEntreReconexoes = default);
        Task ReconectandoAsync(Exception e, int numetoDeTentativas);
    }
}