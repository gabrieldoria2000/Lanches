using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers
{
    public class CarrinhoCompraController : Controller
    {
        private readonly ILancheRepository _lancheRepository;
        private readonly CarrinhoCompra _CarrinhoCompra;

        public CarrinhoCompraController(ILancheRepository lancheRepository, CarrinhoCompra carrinhoCompra)
        {
            _lancheRepository = lancheRepository;
            _CarrinhoCompra = carrinhoCompra;
        }

        public IActionResult Index()
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

        [Authorize]
        public IActionResult AdicionarItemNoCarrinhoCompra(int LancheId)
        {
            var lancheselecionado = _lancheRepository.Lanches.FirstOrDefault(p=> p.LancheId == LancheId);

            if (lancheselecionado != null)
            {
                _CarrinhoCompra.AdicionaAoCarrinho(lancheselecionado);
            }
            return RedirectToAction("Index");   
        }

        [Authorize]
        public IActionResult RemoverItemNoCarrinhoCompra(int LancheId)
        {
            var lancheselecionado = _lancheRepository.Lanches.FirstOrDefault(p => p.LancheId == LancheId);

            if (lancheselecionado != null)
            {
                _CarrinhoCompra.RemoverDoCarrinho(lancheselecionado);
            }
            return RedirectToAction("Index");
        }
    }
}
