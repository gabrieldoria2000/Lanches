using LanchesMac.Models;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Components
{
    public class CarrinhoCompraResumo : ViewComponent
    {
        private readonly CarrinhoCompra _CarrinhoCompra;

        public CarrinhoCompraResumo(CarrinhoCompra carrinhoCompra)
        {
            _CarrinhoCompra = carrinhoCompra;
        }

        public IViewComponentResult Invoke()
        {
            var itens = _CarrinhoCompra.GetCarrinhoCompraItens();
            _CarrinhoCompra.CarrinhoCompraItems = itens;

            var CarrinhoCompraVM = new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _CarrinhoCompra,
                CarrinhoCompraTotal = _CarrinhoCompra.GetCarrinhoComprasTotal()
            };
            return View(CarrinhoCompraVM);
        }
    }
}
