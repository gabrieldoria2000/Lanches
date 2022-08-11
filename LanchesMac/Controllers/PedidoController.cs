using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public PedidoController(IPedidoRepository pedidoRepository, CarrinhoCompra carrinhoCompra)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoCompra = carrinhoCompra;
        }

        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(Pedido pedido)
        {
            int totalItensPedido = 0;
            decimal precoTotalPedido = 0.0m;

            //obtem os itens do carrinho de compra do cliente
            List<CarrinhoCompraItem> itens = _carrinhoCompra.GetCarrinhoCompraItens();

            _carrinhoCompra.CarrinhoCompraItems = itens;

            //verifica se existem itens de pedido
            if(_carrinhoCompra.CarrinhoCompraItems.Count == 0)
            {
                //modelstate é uma coleção de pares x valor que é submetiddo no post
                //ele tem 2 propositos: armazenar os pares x valor e armazenar os erros de validação
                //dessa forma, o modelstate vai ficar num estado inválido. Ou seja, os dados do pedido ficarão num estado invalido
                ModelState.AddModelError("", "Seu carrinho está vazio, que tal incluir um lanche?");
            }
            //calcula o total de itens e o total do pedido
           foreach(var item in itens)
            {
                totalItensPedido += item.Quantidade;
                precoTotalPedido += (item.Quantidade * item.Lanche.Preco);
            }

            //atribui os valores obtidos ao pedido
            pedido.TotalItensPedido = totalItensPedido;
            pedido.PedidoTotal = precoTotalPedido;

            //valida os dados do pedido - verifica as validações
            if (ModelState.IsValid)
            {
                //cria o pedido e os detalhes do pedido
                _pedidoRepository.CriarPedido(pedido);

                //define mensagens ao cliente
                //viewbag - passar dados entre client e controller
                ViewBag.CheckoutCompletoMensagem = "Obrigado pelo seu pedido :)";
                ViewBag.TotalPedido = _carrinhoCompra.GetCarrinhoComprasTotal();

                //limpa o carrinho do cliente
                _carrinhoCompra.LimparCarrinho();

                //exibe a view com dados do cliente e do pedido
                return View("~/Views/Pedido/CheckoutCompleto.cshtml", pedido);
            }

            return View(pedido);

        }
    }
}
