using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ProcessadorPedidos.Domain.Entities;

namespace ProcessadorPedidos.Application.Ouvintes
{
    public interface IOuvintePedidos
    {
        public Task Iniciar(CancellationToken ctoken);
        public ISubject<Pedido> ObterSubject();
    }
}