using dn32.socket.cliente;
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

        public ExemploDeRepresentacaoDeServidorNoCliente() : base(usarCompressao: true)
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

            MensagemDeDebug($"Envaiado para o cliente: 'Teste de envio do servidor'");
            var retorno1 = await EnviarMensagemDeExemplo("Teste de envio do servidor");
            MensagemDeDebug($"Recebi de retorno: '{retorno1.Nome}'");
            Respondido = true;
        }

        public async Task<ExemploDePacoteDeRetorno> EnviarMensagemDeExemplo(string texto)
        {
            var exemploDePacote = new ExemploDePacoteDeEnvio { Informacao = texto };
            return await EnviarMensagemComRetornoAsync<ExemploDePacoteDeRetorno>(exemploDePacote);
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
