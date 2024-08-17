using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using CanguroDTOs.Shared.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

public class SucursalController : Controller
{
    private readonly HttpClient _httpClient;

    public SucursalController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    #region Lista
    public async Task<IActionResult> Index()
    {
        if (!SetAuthorizationHeader())
        {
            return RedirectToAction("Login", "Login");
        }

        var response = await _httpClient.GetAsync("api/Sucursal/Lista");

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var sucursalListResponse = JsonConvert.DeserializeObject<SucursalListResponse>(jsonString);
            var sucursales = sucursalListResponse!.Value;

            var responseMonedas = await _httpClient.GetAsync("api/Moneda/Lista");
            if (responseMonedas.IsSuccessStatusCode)
            {
                var jsonStringMonedas = await responseMonedas.Content.ReadAsStringAsync();
                var monedaListResponse = JsonConvert.DeserializeObject<MonedaListResponse>(jsonStringMonedas);
                var monedas = monedaListResponse!.Value;

                foreach (var sucursal in sucursales)
                {
                    var moneda = monedas.FirstOrDefault(m => m.IdMoneda == sucursal.IdMoneda);
                    sucursal.NombreMoneda = moneda?.Nombre ?? "Moneda no disponible";
                }
            }

            return View(sucursales);
        }

        // Manejo de errores
        ModelState.AddModelError("", "Error al obtener la lista de sucursales.");
        return View(Enumerable.Empty<SucursalDTO>());
    }

    #endregion

    #region Creacion
    public async Task<IActionResult> Create()
    {
        if (!SetAuthorizationHeader())
        {
            return RedirectToAction("Login", "Login");
        }

        List<MonedaDTO> monedas;
        var response = await _httpClient.GetAsync("api/Moneda/Lista");

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var monedaListResponse = JsonConvert.DeserializeObject<MonedaListResponse>(jsonString);
            monedas = monedaListResponse!.Value;
        }
        else
        {
            ModelState.AddModelError("", "Error al obtener la lista de monedas.");
            monedas = new List<MonedaDTO>();
        }

        var viewModel = new SucursalViewModel
        {
            Sucursal = new SucursalDTO(),
            Monedas = new SelectList(monedas, "IdMoneda", "Nombre") 
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SucursalDTO sucursal)
    {
        if (!SetAuthorizationHeader())
        {
            return RedirectToAction("Login", "Login");
        }
        sucursal.FechaCreacion = DateTime.UtcNow;
        var response = await _httpClient.PostAsJsonAsync("api/Sucursal/Crear", sucursal);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", "Error al crear la sucursal.");
        return View(sucursal);
    }
    #endregion

    #region Edicion
    public async Task<IActionResult> Edit(int id)
    {
        if (!SetAuthorizationHeader())
        {
            return RedirectToAction("Login", "Login");
        }

        var responseSucursal = await _httpClient.GetAsync($"api/Sucursal/{id}");

        if (!responseSucursal.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Error al obtener los datos de la sucursal.");
            return RedirectToAction("Index");
        }

        var jsonStringSucursal = await responseSucursal.Content.ReadAsStringAsync();
        var sucursal = JsonConvert.DeserializeObject<SucursalDTO>(jsonStringSucursal);

        var responseMonedas = await _httpClient.GetAsync("api/Moneda/Lista");

        List<MonedaDTO> monedas;
        if (responseMonedas.IsSuccessStatusCode)
        {
            var jsonStringMonedas = await responseMonedas.Content.ReadAsStringAsync();
            var monedaListResponse = JsonConvert.DeserializeObject<MonedaListResponse>(jsonStringMonedas);
            monedas = monedaListResponse!.Value;
        }
        else
        {
            ModelState.AddModelError("", "Error al obtener la lista de monedas.");
            monedas = new List<MonedaDTO>();
        }

        var viewModel = new SucursalViewModel
        {
            Sucursal = sucursal,
            Monedas = new SelectList(monedas, "IdMoneda", "Nombre", sucursal.IdMoneda) 
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SucursalDTO sucursal)
    {
        if (!SetAuthorizationHeader())
        {
            return RedirectToAction("Login", "Login");
        }
        var response = await _httpClient.PutAsJsonAsync($"api/Sucursal/Actualizar/{sucursal.IdSucursal}", sucursal);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        var responseMonedas = await _httpClient.GetAsync("api/Moneda/Lista");
        List<MonedaDTO> monedas;
        if (responseMonedas.IsSuccessStatusCode)
        {
            var jsonStringMonedas = await responseMonedas.Content.ReadAsStringAsync();
            var monedaListResponse = JsonConvert.DeserializeObject<MonedaListResponse>(jsonStringMonedas);
            monedas = monedaListResponse!.Value;
        }
        else
        {
            monedas = new List<MonedaDTO>();
        }

        var viewModel = new SucursalViewModel
        {
            Sucursal = sucursal,
            Monedas = new SelectList(monedas, "IdMoneda", "Nombre", sucursal.IdMoneda) 
        };

        ModelState.AddModelError("", "Error al actualizar la sucursal.");
        return View(viewModel);
    }
    #endregion

    #region Eliminacion
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!SetAuthorizationHeader())
        {
            return RedirectToAction("Login", "Login");
        }


        var response = await _httpClient.DeleteAsync($"api/Sucursal/Eliminar/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", "Error al eliminar la sucursal.");
        return RedirectToAction("Index");
    }
    #endregion

    private bool SetAuthorizationHeader()
    {
        var token = HttpContext.Session.GetString("JwtToken");

        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return true;
    }

    public class SucursalListResponse
    {
        public List<SucursalDTO> Value { get; set; }
    }

    public class MonedaListResponse
    {
        public List<MonedaDTO> Value { get; set; }
    }


}

internal class SucursalViewModel
{
    public SucursalDTO Sucursal { get; set; }
    public SelectList Monedas { get; set; }
}