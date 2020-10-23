using dn32.socket.servidor.Exemplos.Model;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace dn32.socket.servidor.Exemplos
{
    public class ExemploDeRepresentacaoDeServidorNoCliente : DnRepresentanteDoServidorNoCliente
    {
        public Stopwatch Cronometro { get; private set; }

        public bool Respondido { get; set; }

        public ExemploDeRepresentacaoDeServidorNoCliente()
        {
            Cronometro = new Stopwatch(); Cronometro.Start();
        }

        public override Task ConectandoAsync()
        {
            MensagemDeDebug("Conectando");
            return base.ConectandoAsync();
        }

        public override Task ConectadoAsync()
        {
            MensagemDeDebug($"Conectado");
            return base.ConectadoAsync();
        }

        public override async Task DesconectadoAsync(Exception exception)
        {
            MensagemDeDebug($"Desconectado '{exception.Message}'");
            await Task.CompletedTask;
        }

        public async Task Inicializar()
        {
            MensagemDeDebug("Inicializar.");

            await Inicializar("ws://localhost:9008/ws");

            var retorno1 = await EnviarMensagemDeExemplo("Teste de envio");
            Respondido = true;
        }

        public async Task<ExemploDePacoteDeRetorno> EnviarMensagemDeExemplo(string texto)
        {
            var exemploDePacote = new ExemploDePacoteDeEnvio { Informacao = texto };
            return await EnviarMensagem<ExemploDePacoteDeRetorno>(exemploDePacote);
        }

        public override async Task<object> MensagemRecebidaAsync(string mensagem)
        {
            MensagemDeDebug(mensagem);
            await Task.CompletedTask;
            return new ExemploDePacoteDeRetorno
            {
                Nome = "Retorno do cliente"
            };
        }

        private void MensagemDeDebug(string mensagem)
        {
            Console.WriteLine($"{mensagem}. T: {Cronometro.ElapsedMilliseconds}");
        }
    }
}
