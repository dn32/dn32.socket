﻿using System;
using System.Threading;

namespace dn32.socket
{
    internal class RetornoDeMensagem
    {
        public SemaphoreSlim Semaforo { get; set; }
        public string Retorno { get; set; }
        public Guid IdDaRequisicao { get; set; }
    }
}
