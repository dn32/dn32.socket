using dn32.socket.servidor.Exemplos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dn32.socket.cliente
{
    class Program
    {
        static void Main(string[] _)
        {

            MainAsync();

            Console.ReadKey();
        }

        const int QUANTIDADE = 1;

        static async void MainAsync()
        {
            var t1 = new Stopwatch(); t1.Start();
            Console.WriteLine("Iniciando");
            var lista = new List<ExemploDeRepresentacaoDeServidorNoCliente>();

            for (int i = 0; i < QUANTIDADE; i++)
            {
                _ = Task.Run(async () =>
                {
                    var represent = new ExemploDeRepresentacaoDeServidorNoCliente();
                    await represent.Inicializar();
                    lock (lista) lista.Add(represent);
                });
            }

            while (lista.Count < QUANTIDADE || lista.Any(x => !x.Respondido))
            {
                await Task.Delay(10);
            }
            t1.Stop();

            Console.WriteLine($"{lista.Count} Finalizado em {t1.ElapsedMilliseconds}");

           // await Task.Delay(10 * 1000);

            //foreach (var cliente in lista)
            //{
            //    cliente.Desconectar();
            //}
            //Console.WriteLine($"Desconectado em {t1.ElapsedMilliseconds}");
        }
    }
}
