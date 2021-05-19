using System;
using System.Threading;

namespace dn32.socket
{
    internal class RetornoDeMensagem : IDisposable
    {
        public SemaphoreSlim Semaforo { get; set; }
        public bool Disposed { get; set; }
        public string Retorno { get; set; }
        public Guid IdDaRequisicao { get; set; }

        public void Dispose()
        {
            Disposed = true;
            Semaforo.Dispose();
        }
    }
}
