using LanchesMac.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LanchesMac.ViewModels;

namespace LanchesMac.Controllers
{
    public class LancheController : Controller
    {
        private readonly ILancheRepository _lancheRepository;

        public LancheController(ILancheRepository lancheRepository)
        {
            _lancheRepository = lancheRepository;
        }

        public IActionResult List()
        {
            /*ViewData["Titulo"] = "Todos os Lanches";
           ViewData["Data"] = DateTime.Now;

            
            var totalLanches = lanches.Count();

            ViewBag.Total = "Total de Lanches : ";
            ViewBag.TotalLanches = totalLanches;

            TempData["Nome"] = "DoriaLanches";
            */
            //var lanches = _lancheRepository.Lanches;
            //return View(lanches);

            var LancheListViewModel = new LancheListViewModel();
            LancheListViewModel.Lanches = _lancheRepository.Lanches;
            LancheListViewModel.CategoriaAtual = "Categorial Atual";

            return View(LancheListViewModel);


        }
    }
}
