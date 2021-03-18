using System;
using System.Threading.Tasks;
using dn32.socket.servidor.Exemplos.Model;

namespace dn32.socket.servidor.Exemplos
{
    public class TarefaEmSegundoPlano
    {
        public ExemploDeRepresentacaoDeClienteNoServidor Cliente { get; set; }

        public async Task Iniciar(ExemploDeRepresentacaoDeClienteNoServidor cliente)
        {
            await Task.CompletedTask;

            Cliente = cliente;

            //int i = 0;
            //while (!Cliente.Ctoken.IsCancellationRequested)
            //{
            //    i++;
            //    await Task.Delay(2000);

            //    var retorno = await Cliente.EnviarMensagem<ExemploDePacoteDeRetorno>(new ExemploDePacoteDeEnvio
            //    {
            //        Informacao = "Envio periódico do servidor"
            //    });

            //    Console.WriteLine(retorno.Nome);

            //if (i == 5)
            //{
            //await Task.Delay(1 * 1000);
            //await Task.Delay(1 * 1000);
            //await cliente.Desconectar();
            //  break;
            // }
            //}
        }
    }
}
