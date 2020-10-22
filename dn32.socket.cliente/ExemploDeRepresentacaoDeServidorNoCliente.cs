using dn32.socket.servidor.Exemplos.Model;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace dn32.socket.servidor.Exemplos
{
    public class ExemploDeRepresentacaoDeServidorNoCliente : RepresentanteDoServidorNoCliente
    {
        public Stopwatch Cronometro { get; private set; }

        public bool Respondido { get; set; }

        public ExemploDeRepresentacaoDeServidorNoCliente()
        {
            Cronometro = new Stopwatch(); Cronometro.Start();
        }

        public override Task Conectando()
        {
            MensagemDeDebug("Conectando");
            return base.Conectando();
        }

        public override Task Conectado()
        {
            MensagemDeDebug($"Conectado");
            return base.Conectado();
        }

        public async Task Inicializar()
        {
            MensagemDeDebug("Inicializar.");

            var tratador = new ExemploDeRepresentacaoDeServidorNoCliente();
            await tratador.Inicializar("ws://localhost:9008/ws");

            var exemploDePacote = new ExemploDePacoteDeEnvio
            {
                Informacao = "Teste de envio"
            };

            var retorno = await tratador.EnviarMensagem<ExemploDePacoteDeRetorno>(exemploDePacote);
            MensagemDeDebug("Retorno: " + retorno.Nome);
            Respondido = true;
        }

        public override async Task<object> MensagemRecebida(string mensagem)
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
