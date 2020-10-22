using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
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

        static async void MainAsync()
        {
            var t1 = new Stopwatch(); t1.Start();
            Console.WriteLine("Iniciando");
            var lista = new List<Dn32SocketCliente>();

            for (int i = 0; i < 100; i++)
            {
                var dn32SocketCliente = new Dn32SocketCliente();
                await dn32SocketCliente.IniciarComunicacao();
                lista.Add(dn32SocketCliente);
            }

            while(lista.Any(x => !x.Respondido))
            {
                await Task.Delay(1);
            }

            Console.WriteLine($"{lista.Count} Finalizado em {t1.ElapsedMilliseconds}");
        }
    }

    public class Dn32SocketCliente
    {
        public bool Respondido { get; set; }

        private ClientWebSocket ClientWebSocket { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public async Task IniciarComunicacao()
        {
            CancellationTokenSource = new CancellationTokenSource();
            ClientWebSocket = await Conectar("ws://localhost:9008/ws");
            MensagemDeDebug("Conectado.");

            var t1 = new Stopwatch(); t1.Start();

            _ = Util.ReceberAsync(ClientWebSocket, CancellationTokenSource.Token,
                            async (ContratoDeMensagem mensagem) =>
                            {
                                Util.MensagemDeDebug(mensagem.Conteudo);
                                Util.MensagemDeDebug(t1.ElapsedMilliseconds.ToString());
                                await Task.CompletedTask;
                                Respondido = true;
                                return null;
                            });

            await Util.EnviarMensagem(ClientWebSocket, CancellationTokenSource.Token, "Olá teste!");
        }

        public async Task<ClientWebSocket> Conectar(string url)
        {
            var webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri(url), CancellationTokenSource.Token);
            return webSocket;
        }

        private void MensagemDeDebug(string mensagem)
        {
            Console.WriteLine(mensagem);
        }
    }
}
