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

        public override async Task Desconectado(Exception exception)
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
