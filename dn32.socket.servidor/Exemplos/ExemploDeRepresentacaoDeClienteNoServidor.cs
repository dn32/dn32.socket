using System;
using System.Threading.Tasks;
using dn32.socket.servidor.Exemplos.Model;
using Newtonsoft.Json;

namespace dn32.socket.servidor.Exemplos
{
    public class ExemploDeRepresentacaoDeClienteNoServidor : DnRepresentacaoDoClienteNoServidor
    {
        public TarefaEmSegundoPlano TarefaEmSegundoPlano { get; set; }

        public override Task Conectando()
        {
            MensagemDeDebug("Conectando");
            return base.Conectando();
        }

        public override Task Conectado()
        {
            MensagemDeDebug($"Conectado");
            TarefaEmSegundoPlano = new TarefaEmSegundoPlano();
            _ = TarefaEmSegundoPlano.Iniciar(this);
            return base.Conectado();
        }

        public override async Task Desconectado(Exception exception)
        {
            MensagemDeDebug($"Desconectado '{exception.Message}'");
            await Task.CompletedTask;
        }

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

        private void MensagemDeDebug(string mensagem)
        {
            Console.WriteLine($"{mensagem}.");
        }
    }
}
