using CanguroDTOs.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; 
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CanguroCRUD.Client.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("api/Acceso/Login", model);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);
                string token = tokenResponse!.Token;

                // Almacenar el token en la sesión
                HttpContext.Session.SetString("JwtToken", token);

               

                return RedirectToAction("Index", "Sucursal");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError("", "Usuario o clave inválidos.");
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                ModelState.AddModelError("", "Solicitud incorrecta. Verifica tu entrada.");
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                ModelState.AddModelError("", "Acceso denegado.");
            }
            else
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado.");
            }

            return View(model);
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
    }
}
