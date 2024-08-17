using CanguroAPI.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CanguroAPI.Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MonedaController : ControllerBase
    {

        private readonly DbpruebaCanguroContext _dbpruebaCanguroContext;

        public MonedaController(DbpruebaCanguroContext dbpruebaCanguroContext)
        {
            _dbpruebaCanguroContext = dbpruebaCanguroContext;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            await RegistrarLog("Solicitud para obtener la lista de monedas recibida.", "INFO");

            var lista = await _dbpruebaCanguroContext.Moneda.ToListAsync();

            await RegistrarLog($"Se han recuperado {lista.Count} monedas.", "INFO");

            return StatusCode(StatusCodes.Status200OK, new { Value = lista });
        }

        private async Task RegistrarLog(string mensaje, string nivel, int? userId = null)
        {
            Log log = new Log
            {
                LogDate = DateTime.UtcNow,
                LogLevel = nivel,
                Description = mensaje,
                UserId = userId,
                ControllerName = ControllerContext.ActionDescriptor.ControllerName,
                ActionName = ControllerContext.ActionDescriptor.ActionName,
                RequestUrl = HttpContext.Request.Path
            };

            await _dbpruebaCanguroContext.Logs.AddAsync(log);
            await _dbpruebaCanguroContext.SaveChangesAsync();
        }
    }
}
