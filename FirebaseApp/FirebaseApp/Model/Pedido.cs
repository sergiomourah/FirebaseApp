using System;
using System.Collections.Generic;
using System.Text;

namespace FirebaseApp.Model
{
    public class Pedido
    {
        public string KeyPedido { get; set; }

        public string Cliente { get; set; }

        public decimal Valor { get; set; }

        public string Produto { get; set; }

        public int IdVendedor { get; set; }
    }
}
