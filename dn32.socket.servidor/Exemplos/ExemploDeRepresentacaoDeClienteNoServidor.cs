using System;
using System.Threading.Tasks;
using dn32.socket.servidor.Exemplos.Model;
using Newtonsoft.Json;

namespace dn32.socket.servidor.Exemplos
{
    public class ExemploDeRepresentacaoDeClienteNoServidor : DnRepresentacaoDoClienteNoServidor
    {
        public TarefaEmSegundoPlano TarefaEmSegundoPlano { get; set; }

        public override Task ConectadoAsync()
        {
            MensagemDeDebug($"Conectado");
            TarefaEmSegundoPlano = new TarefaEmSegundoPlano();
            _ = TarefaEmSegundoPlano.Iniciar(this);
            return base.ConectadoAsync();
        }

        public override async Task DesconectadoAsync(Exception exception)
        {
            MensagemDeDebug($"Desconectado '{exception.Message}'");
            await Task.CompletedTask;
        }

        public override async Task<object> MensagemRecebidaAsync(string mensagem)
        {
            await Task.CompletedTask;

            var pacote = JsonConvert.DeserializeObject<ExemploDePacoteDeEnvio>(mensagem);
            Console.WriteLine(pacote.Informacao);

            return new ExemploDePacoteDeRetorno
            {
                Nome = "Zé Piqueno"
            };
        }

        private void MensagemDeDebug(string mensagem)
        {
            Console.WriteLine($"{mensagem}.");
        }
    }
}
