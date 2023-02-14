using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationMostrar.Models;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;



namespace WebApplicationMostrar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private List<CryptocurrencyWithLatestQuote> datos;
        CoinMarketCapClient cliente = new CoinMarketCapClient();

        public  HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            
            datos = cliente.GetLatestListings(new ListingLatestParameters { Limit = 10 }).Data;
        }

        public IActionResult Index()
        {
            // List<CryptocurrencyWithLatestQuote> model = ;
            //GetLatestListingsAsync_GivenRequest_Succeeds();
            CriptoConversion model = new CriptoConversion(datos,0);
            return View(model);
        }

        public JsonResult ActualizacionCotizaciones()
        {

            CriptoConversion model = new CriptoConversion(datos, 0);
            return Json(model);
        }

        [HttpPost]
        public IActionResult ActualizacionConversiones(string IdLstCritomoneda, string IdCantidad)
        {
            CriptoConversion model = new CriptoConversion(datos, Double.Parse(IdCantidad));
            model.IdLstCritomoneda = IdLstCritomoneda;
            return View("Index", model);



        }

        public PartialViewResult CotizacionesView()
        {
            // List<CryptocurrencyWithLatestQuote> model = ;
            datos = cliente.GetLatestListings(new ListingLatestParameters { Limit = 10 }).Data;
            CriptoConversion model = new CriptoConversion(datos, 10);
            return PartialView(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}