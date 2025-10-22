using Microsoft.AspNetCore.Mvc;
using consensoria_autos.Models;
using consensoria_autos.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace consensoria_autos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiClient _api;

        public HomeController(ApiClient api)
        {
            _api = api;
        }

        // ===========================================================
        //                    HOME PAGE 
        // ===========================================================
        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;

            var response = await _api.GetAsync<List<Car>>("cars");

       

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = $"❌ Error al cargar vehículos: {response.Message}";
                return View(new List<Car>());
            }

            var cars = response.Data;

            if (!string.IsNullOrWhiteSpace(search))
                cars = cars.FindAll(c => c.model.ToLower().Contains(search.ToLower()));

            return View(cars);
        }


        // ===========================================================
        //                    DETALLES DEL AUTO
        // ===========================================================
        public async Task<IActionResult> Details(int id)
        {
            var response = await _api.GetAsync<Car>($"cars/{id}");

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = "❌ El vehículo no fue encontrado o ocurrió un error al obtenerlo.";
                return RedirectToAction("Index");
            }

            return View(response.Data);
        }

        // ===========================================================
        //                     CONTACTO GENERAL
        // ===========================================================
        [HttpGet]
        public IActionResult Contact() => View();

        [HttpPost]
        
        public async Task<IActionResult> Contact(Message model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "⚠️ Por favor, completa los campos requeridos.";
                return View(model);
            }

            var response = await _api.PostAsync<Message>("messages", model);

            if (response.Success)
            {
                TempData["Success"] = "✅ Tu mensaje ha sido enviado correctamente. ¡Gracias por contactarnos!";
                return RedirectToAction("Contact");
            }

            ViewBag.Error = $"❌ Error al enviar el mensaje: {response.Message}";
            return View(model);
        }

        // ===========================================================
        //                CONTACTAR VENDEDOR POR AUTO
        // ===========================================================
        [HttpGet]
        public async Task<IActionResult> ContactSeller(int id)
        {
            var response = await _api.GetAsync<Car>($"cars/{id}");

            if (!response.Success || response.Data == null)
            {
                TempData["Error"] = "❌ El vehículo no fue encontrado.";
                return RedirectToAction("Index");
            }

            ViewData["Title"] = "Contactar vendedor";
            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> ContactSeller(int id, Message model)
        {
            var responseCar = await _api.GetAsync<Car>($"cars/{id}");
            if (!responseCar.Success || responseCar.Data == null)
            {
                TempData["Error"] = "❌ El vehículo no existe o no pudo cargarse.";
                return RedirectToAction("Index");
            }

            model.content = $"(Vehículo: {responseCar.Data.model}) {model.content}";
            model.car_id = id;

            var responseMsg = await _api.PostAsync<Message>("messages", model);

            if (responseMsg.Success)
            {
                TempData["Success"] = "✅ Tu mensaje ha sido enviado al vendedor.";
                return RedirectToAction("ContactSeller", new { id });
            }

            ViewBag.Error = $"❌ Error al enviar mensaje al vendedor: {responseMsg.Message}";
            return View(responseCar.Data);
        }
    }
}
