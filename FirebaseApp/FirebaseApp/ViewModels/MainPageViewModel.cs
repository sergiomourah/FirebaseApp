using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using FirebaseApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace FirebaseApp.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly string ENDERECO_FIREBASE = "https://demoapp-18599.firebaseio.com/";

        private ObservableCollection<Pedido> _pedidos;

        public ObservableCollection<Pedido> Pedidos
        {
            get { return _pedidos; }
            set { _pedidos = value; OnPropertyChanged(); }
        }

        private Pedido _pedidoSelecionado;

        public Pedido PedidoSelecionado
        {
            get { return _pedidoSelecionado; }
            set { _pedidoSelecionado = value; OnPropertyChanged(); }
        }
        public ICommand AceitarPedidoCmd { get; set; }

        private readonly FirebaseClient _firebaseClient;

        public MainPageViewModel()
        {
            _firebaseClient = new FirebaseClient(ENDERECO_FIREBASE);
            Pedidos = new ObservableCollection<Pedido>();
            AceitarPedidoCmd = new Command(() => AceitarPedido());
            if (PedidoSelecionado == null)
                ListenerPedidos();
        }

        private void ListenerPedidos()
        {
            _pedidos.Clear();
            _firebaseClient
                .Child("pedidos")
                 .AsObservable<Pedido>()
                .Subscribe(d =>
                {
                    if (d.EventType == FirebaseEventType.InsertOrUpdate)
                    {
                        if (d.Object.IdVendedor == 0)
                            AdicionarPedido(d.Key, d.Object);
                        else
                            RemoverPedido(d.Key);

                    }
                    else if (d.EventType == FirebaseEventType.Delete)
                    {
                        RemoverPedido(d.Key);
                    }
                });
        }

        private void AdicionarPedido(string key, Pedido pedido)
        {
            _pedidos.Add(new Pedido()
            {
                KeyPedido = key,
                Cliente = pedido.Cliente,
                Produto = pedido.Produto,
                Valor = pedido.Valor
            });
        }

        private void RemoverPedido(string pedidoKey)
        {
            var pedido = Pedidos.FirstOrDefault(x => x.KeyPedido == pedidoKey);
            Pedidos.Remove(pedido);
        }

        private void AceitarPedido()
        {
            if (_pedidoSelecionado != null)
            {
                _pedidoSelecionado.IdVendedor = 1;

                _firebaseClient
                    .Child("pedidos")
                    .Child(_pedidoSelecionado.KeyPedido)
                    .PutAsync(_pedidoSelecionado);
            }
        }
    }
}
