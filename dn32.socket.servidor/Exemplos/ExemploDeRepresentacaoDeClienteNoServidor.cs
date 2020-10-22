using System;
using System.Threading.Tasks;
using dn32.socket.servidor.Exemplos.Model;
using Newtonsoft.Json;

namespace dn32.socket.servidor.Exemplos
{
    public class ExemploDeRepresentacaoDeClienteNoServidor : RepresentacaoDoClienteNoServidor
    {
        public override async Task<object> MensagemRecebida(string mensagem)
        {
            await Task.CompletedTask;

            var pacote = JsonConvert.DeserializeObject<ExemploDePacoteDeEnvio>(mensagem);
            Console.WriteLine(pacote.Informacao);

            return new ExemploDePacoteDeRetorno
            {
                Nome = "Zé Piqueno"
            };
        }
    }
}
