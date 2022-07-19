﻿using LanchesMac.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LanchesMac.ViewModels;
using LanchesMac.Models;

namespace LanchesMac.Controllers
{
    public class LancheController : Controller
    {
        private readonly ILancheRepository _lancheRepository;

        public LancheController(ILancheRepository lancheRepository)
        {
            _lancheRepository = lancheRepository;
        }

        public IActionResult List(string categoria)
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

            //var LancheListViewModel = new LancheListViewModel();
            //LancheListViewModel.Lanches = _lancheRepository.Lanches;
            //LancheListViewModel.CategoriaAtual = "Categorial Atual";

            IEnumerable<Lanche> lanches;
            string categoriaAtual = null;

            if (string.IsNullOrEmpty(categoria))
            {
                lanches = _lancheRepository.Lanches.OrderBy(l => l.LancheId);
                categoriaAtual = "Todos os Lanches";
            }
            else
            {
                if(string.Equals("Normal", categoria, StringComparison.OrdinalIgnoreCase))
                {
                    lanches = _lancheRepository.Lanches.Where(l => l.Categoria.CategoriaNome.Equals("Normal"))
                        .OrderBy(l => l.Nome);
                }
                else
                {
                    lanches = _lancheRepository.Lanches.Where(l => l.Categoria.CategoriaNome.Equals("Natural"))
                        .OrderBy(l => l.Nome);
                }
                categoriaAtual = categoria;
            }

            var lanchesListViewModel = new LancheListViewModel
            {
                Lanches = lanches,
                CategoriaAtual = categoriaAtual
            };

            return View(lanchesListViewModel);


        }
    }
}
