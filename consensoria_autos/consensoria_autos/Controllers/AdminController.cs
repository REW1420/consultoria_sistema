using Microsoft.AspNetCore.Mvc;
using consensoria_autos.Models;
using consensoria_autos.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace consensoria_autos.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiClient _api;
        private readonly HttpClient _http = new() { BaseAddress = new Uri("http://localhost:3000/api/") };

        public AdminController(ApiClient api)
        {
            _api = api;
        }

        // ===========================================================
        // MÉTODO DE VALIDACIÓN DE SESIÓN
        // ===========================================================
        private bool EnsureAuthenticated(out IActionResult redirect)
        {
            var token = HttpContext.Session.GetString("token");
            var role = HttpContext.Session.GetString("role");

            if (string.IsNullOrEmpty(token))
            {
                redirect = RedirectToAction("Login");
                return false;
            }

            if (role != "Admin" && role != "Sales")
            {
                redirect = RedirectToAction("Login");
                return false;
            }

            redirect = null!;
            return true;
        }

        // ===========================================================
        // LOGIN Y SESIÓN
        // ===========================================================
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            Console.WriteLine($"🧠 Login recibido: {username}:{password}");

            var response = await _http.PostAsJsonAsync("auth/login", new { username, password });
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Credenciales inválidas.";
                return View();
            }

            var result = JsonSerializer.Deserialize<AuthResponse>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            HttpContext.Session.SetString("token", result!.Token);
            HttpContext.Session.SetString("role", result!.User.Role);
            HttpContext.Session.SetString("username", result!.User.Username);

            Console.WriteLine($"✅ Login exitoso: {result.User.Username} ({result.User.Role})");

            return RedirectToAction("Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ===========================================================
        // DASHBOARD
        // ===========================================================
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            Console.WriteLine("📥 [Dashboard] Solicitando mensajes...");

            // 🔹 Ahora GetAsync devuelve directamente List<Message>
            var messages = await _api.GetAsync2<List<Message>>("messages");

            if (messages == null)
            {
                TempData["Error"] = "❌ No se pudieron obtener los mensajes.";
                return View("Dashboard", new List<Message>());
            }

            Console.WriteLine($"✅ Se recibieron {messages.Count} mensajes del API.");

            var sorted = messages.OrderByDescending(m => m.received_at).ToList();

            return View("Dashboard", sorted);
        }






        // ===========================================================
        // MENSAJES CRUD
        // ===========================================================
        [HttpGet]
        public async Task<IActionResult> ViewMessage(int id)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var message = await _api.GetAsync<Message>($"messages/{id}");
            if (message.Data == null) return NotFound();

            await _api.PutAsync<Message>($"messages/{id}/read", new { });
            return View("ViewMessage", message.Data);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var result = await _api.PutAsync<Message>($"messages/{id}/read", new { });
            TempData[result.Success ? "Success" : "Error"] =
                result.Success ? "Mensaje marcado como leído." : $"Error: {result.Message}";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var result = await _api.DeleteAsync($"messages/{id}");
            TempData[result.Success ? "Success" : "Error"] =
                result.Success ? "Mensaje eliminado correctamente." : $"Error: {result.Message}";
            return RedirectToAction("Dashboard");
        }

        // ===========================================================
        // AUTOS CRUD
        // ===========================================================
        [HttpGet]
        public async Task<IActionResult> AutoList(string? search)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var cars = await _api.GetAsync<List<Car>>("cars");
            var filtered = cars.Data ?? new();

            if (!string.IsNullOrWhiteSpace(search))
                filtered = filtered.FindAll(c => c.model.ToLower().Contains(search.ToLower()));

            return View(filtered);
        }

        [HttpGet]
        public async Task<IActionResult> AddAuto()
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var transmissions = await _api.GetAsync<List<Transmission>>("transmissions");
            var conditions = await _api.GetAsync<List<CarCondition>>("conditions");
            ViewBag.Transmissions = transmissions.Data ?? new();
            ViewBag.Conditions = conditions.Data ?? new();
            return View("AddAuto", new Car());
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddAuto(Car car)
        {
            try
            {
                // 🧩 Asegurar que price sea null si viene vacío
                if (car.price.HasValue && car.price.Value == 0)
                {
                    Console.WriteLine("⚠️ Precio 0 detectado — se mantiene en 0.");
                }
                else if (car.price == null || car.price == 0)
                {
                    car.price = null; // 🔥 evita enviar "" o 0 si no hay valor real
                }

                List<string>? imageList = null;
                if (!string.IsNullOrEmpty(car.images))
                    imageList = JsonSerializer.Deserialize<List<string>>(car.images);

                var payload = new
                {
                    car.model,
                    car.description,
                    car.year,
                    car.price, // ahora seguro es number o null
                    car.is_published,
                    car.transmission_id,
                    car.condition_id,
                    images = imageList
                };

                Console.WriteLine("📤 Enviando payload:");
                Console.WriteLine(JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));

                var response = await _api.PostAsync<ApiResponse<Car>>("cars", payload);

                if (response != null && response.Success)
                {
                    TempData["Success"] = "🚗 Vehicle added successfully!";
                    return RedirectToAction("AutoList");
                }

                TempData["Error"] = $"❌ Failed to add vehicle: {response?.Message ?? "Unknown error"}";
                return View("AddAuto", car);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error AddAuto: {ex.Message}");
                TempData["Error"] = "Error adding vehicle.";
                return View("AddAuto", car);
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditAuto(int id)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var car = await _api.GetAsync<Car>($"cars/{id}");
            var transmissions = await _api.GetAsync<List<Transmission>>("transmissions");
            var conditions = await _api.GetAsync<List<CarCondition>>("conditions");

            ViewBag.Transmissions = transmissions.Data ?? new();
            ViewBag.Conditions = conditions.Data ?? new();

            return View("EditAuto", car.Data);
        }

        [HttpPost]
        public async Task<IActionResult> EditAuto(Car car)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            Console.WriteLine($"🚗 [EditAuto] Intentando actualizar vehículo ID = {car.id}");

            try
            {
              

                // 🔹 2. Enviar PUT al backend
                var response = await _api.PutAsync<ApiResponse<Car>>($"cars/{car.id}", car);

                // 🔹 3. Validar respuesta nula o fallida
                if (response == null)
                {
                    Console.WriteLine($"❌ [API] No se recibió respuesta del servidor o el formato fue inválido.");
                    TempData["Error"] = "Error al comunicarse con el servidor.";
                    return View("EditAuto", car);
                }

               
                if (response.Success)
                {
                    TempData["Success"] = "✅ Vehicle updated successfully.";
                    return RedirectToAction("AutoList");
                }

                TempData["Error"] = $"❌ Error updating vehicle: {response.Message ?? "Unknown error"}";
                return View("EditAuto", car);
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"🌐 Error HTTP: {httpEx.Message}");
                TempData["Error"] = "No se pudo contactar con el servidor.";
                return View("EditAuto", car);
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"💥 Error de deserialización JSON: {jsonEx.Message}");
                TempData["Error"] = "El servidor devolvió un formato de datos inválido.";
                return View("EditAuto", car);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Error inesperado en EditAuto: {ex.Message}");
                TempData["Error"] = "Ocurrió un error inesperado al actualizar el vehículo.";
                return View("EditAuto", car);
            }
        }


        [HttpPost]
        public async Task<IActionResult> MarkSold(int id)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var result = await _api.PutAsync<Car>($"cars/{id}/sold", new { });

            TempData[result.Success ? "Success" : "Error"] =
                result.Success ? "🚗 Vehicle marked as sold." : $"❌ Error marking as sold: {result.Message}";
            return RedirectToAction("AutoList");
        }

        [HttpPost]
        public async Task<IActionResult> PauseCar(int id)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var result = await _api.PutAsync<Car>($"cars/{id}/publish", new { publish = false });
            TempData[result.Success ? "Success" : "Error"] =
                result.Success ? "⏸ Vehicle paused." : $"❌ Error pausing vehicle: {result.Message}";
            return RedirectToAction("AutoList");
        }

        [HttpPost]
        public async Task<IActionResult> ResumeCar(int id)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var result = await _api.PutAsync<Car>($"cars/{id}/publish", new { publish = true });
            TempData[result.Success ? "Success" : "Error"] =
                result.Success ? "▶ Vehicle reactivated." : $"❌ Error reactivating vehicle: {result.Message}";
            return RedirectToAction("AutoList");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAuto(int id)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var result = await _api.DeleteAsync($"cars/{id}");
            TempData[result.Success ? "Success" : "Error"] =
                result.Success ? "Vehículo eliminado correctamente." : $"❌ Error eliminando vehículo: {result.Message}";
            return RedirectToAction("AutoList");
        }

        // ===========================================================
        // USUARIOS CRUD
        // ===========================================================
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var role = HttpContext.Session.GetString("role");
            if (role != "Admin") return RedirectToAction("Dashboard");

            var users = await _api.GetAsync<List<User>>("users");
            return View(users.Data ?? new List<User>());
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var roles = await _api.GetAsync<List<UserRole>>("roles");
            var user = await _api.GetAsync<User>($"users/{id}");
            if (user.Data == null) return NotFound();

            ViewBag.Roles = roles.Data ?? new();
            return View("EditUser", user.Data);
        }

        [HttpGet]
        public async Task<IActionResult> AddUser(int? id)
        {
            var roles = await _api.GetAsync<List<UserRole>>("roles");
            ViewBag.Roles = roles.Data ?? new();

            if (id.HasValue)
            {
                var existing = await _api.GetAsync<User>($"users/{id}");
                if (existing.Data == null) return NotFound();

                ViewBag.IsEdit = true;
                return View("EditUser", existing.Data);
            }

            ViewBag.IsEdit = false;
            return View("AddUser", new User());
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User model)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Verifica los campos antes de guardar.";
                var roles = await _api.GetAsync<List<UserRole>>("roles");
                ViewBag.Roles = roles.Data ?? new();
                return View("EditUser", model);
            }

            var response = await _api.PutAsync<User>($"users/{model.id}", model);

            if (response.Success)
            {
                TempData["Success"] = $"✅ Usuario '{model.username}' actualizado correctamente.";
                return RedirectToAction("Users");
            }

            TempData["Error"] = $"❌ No se pudo actualizar el usuario: {response.Message}";
            return View("EditUser", model);
        }

        [HttpPost, ActionName("AddUser")]
        public async Task<IActionResult> AddUserPost(User model)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Verifica los campos e inténtalo de nuevo.";
                return View("AddUser", model);
            }

            var result = model.id > 0
                ? await _api.PutAsync<User>($"users/{model.id}", model)
                : await _api.PostAsync<User>("users", model);

            if (result.Success)
            {
                TempData["Success"] = $"✅ Usuario '{model.username}' guardado correctamente.";
                return RedirectToAction("Users");
            }

            TempData["Error"] = $"❌ Error al guardar usuario: {result.Message}";
            return View(model.id > 0 ? "EditUser" : "AddUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActiveUser(int id)
        {
            var user = await _api.GetAsync<User>($"users/{id}");
            if (user.Data == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Users");
            }

            user.Data.active = !(user.Data.active ?? false);
            var result = await _api.PutAsync<User>($"users/{id}", user.Data);

            TempData[result.Success ? "Success" : "Error"] =
                result.Success
                ? $"Usuario {(user.Data.active == true ? "activado" : "desactivado")} correctamente."
                : $"❌ No se pudo actualizar el estado: {result.Message}";

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!EnsureAuthenticated(out var redirect))
                return redirect;

            var result = await _api.DeleteAsync($"users/{id}");
            TempData[result.Success ? "Success" : "Error"] =
                result.Success ? "Usuario eliminado correctamente." : $"❌ Error eliminando usuario: {result.Message}";

            return RedirectToAction("Users");
        }
    }
}
