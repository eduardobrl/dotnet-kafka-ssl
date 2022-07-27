using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessadorPedidos.Domain.Entities
{
    public class Pedido
    {
        public string? SKU { get; set; }
        public string? Nome { get; set; }
        public decimal Valor { get; set; }
        public string? Fornecedor { get; set; }
        public string? Vendedor { get; set; }
    }
}