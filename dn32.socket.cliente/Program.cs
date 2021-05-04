using dn32.socket.servidor;
using dn32.socket.servidor.Exemplos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.socket.cliente
{
    class Program
    {
        private static SemaphoreSlim ControleDeFluxo { get; set; }
        private const int QUANTIDADE = 1;

        static async void MainAsync()
        {
            Console.WriteLine("Aguardando...");
            await Task.Delay(2000);
            Console.WriteLine("Iniciando");
            var t1 = new Stopwatch(); t1.Start();
            var lista = new List<ExemploDeRepresentacaoDeServidorNoCliente>();

            ControleDeFluxo = new SemaphoreSlim(20);

            for (int i = 0; i < QUANTIDADE; i++)
            {
                _ = Task.Run(async () =>
                {
                    await ControleDeFluxo.WaitAsync();
                    var represent = new ExemploDeRepresentacaoDeServidorNoCliente();
                    await represent.Inicializar();
                    lock (lista) lista.Add(represent);
                    ControleDeFluxo.Release();
                });
            }

            while (lista.Count < QUANTIDADE || lista.Any(x => !x.Respondido))
            {
                await Task.Delay(10);
            }
            t1.Stop();

            Console.WriteLine($"{lista.Count} Finalizado em {t1.ElapsedMilliseconds}");

            await Task.Delay(1 * 1000);

            //foreach (var cliente in lista)
            //{
            //    await cliente.Desconectar();
            //}

            //Console.WriteLine($"Desconectado em {t1.ElapsedMilliseconds}");
        }

        static void Main(string[] _)
        {
            MainAsync();
            Console.ReadKey();
        }
    }
}
