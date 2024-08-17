using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CanguroAPI.Server.Custom;
using CanguroAPI.Server.Models;
using CanguroDTOs.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace CanguroAPI.Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SucursalController : ControllerBase
    {
        private readonly DbpruebaCanguroContext _dbpruebaCanguroContext;

        public SucursalController(DbpruebaCanguroContext dbpruebaCanguroContext)
        {
            _dbpruebaCanguroContext = dbpruebaCanguroContext;
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            await RegistrarLog("Solicitud para obtener la lista de sucursales recibida.", "INFO");

            var lista = await _dbpruebaCanguroContext.Sucursals.ToListAsync();

            await RegistrarLog($"Se han recuperado {lista.Count} sucursales.", "INFO");

            return StatusCode(StatusCodes.Status200OK, new {Value = lista});
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var sucursal = await _dbpruebaCanguroContext.Sucursals.FindAsync(id);

            if (sucursal == null)
            {
                await RegistrarLog($"Sucursal con ID {id} no encontrada.", "ERROR");
                return new JsonResult(new { Error = "Sucursal no encontrada" }) { StatusCode = StatusCodes.Status404NotFound };
            }

            return new JsonResult(sucursal);
        }

        [HttpPost]
        [Route("Crear")]
        public async Task<IActionResult> Crear( SucursalDTO sucursalDTO)
        {
            if (sucursalDTO == null)
            {
                return BadRequest("Datos inválidos.");
            }

            var sucursal = new Sucursal
            {
                Descripcion = sucursalDTO.Descripcion,
                Direccion = sucursalDTO.Direccion,
                Identificacion = sucursalDTO.Identificacion,
                FechaCreacion = sucursalDTO.FechaCreacion,
                IdMoneda = sucursalDTO.IdMoneda
            };

            await _dbpruebaCanguroContext.Sucursals.AddAsync(sucursal);
            await _dbpruebaCanguroContext.SaveChangesAsync();

            await RegistrarLog($"Sucursal con ID {sucursal.IdSucursal} creada.", "INFO");
            return StatusCode(StatusCodes.Status201Created, new { Value = sucursal });
        }

        [HttpPut]
        [Route("Actualizar/{id}")]
        public async Task<IActionResult> Actualizar(int id, SucursalDTO sucursalDto)
        {
            var sucursal = await _dbpruebaCanguroContext.Sucursals.FindAsync(id);

            if (sucursal == null)
            {
                await RegistrarLog($"Sucursal con ID {id} no encontrada para actualización.", "ERROR");
                return NotFound();
            }

            sucursal.Descripcion = sucursalDto.Descripcion;
            sucursal.Direccion = sucursalDto.Direccion;
            sucursal.Identificacion = sucursalDto.Identificacion;
            sucursal.IdMoneda = sucursalDto.IdMoneda;

            _dbpruebaCanguroContext.Sucursals.Update(sucursal);
            await _dbpruebaCanguroContext.SaveChangesAsync();

            await RegistrarLog($"Sucursal con ID {id} actualizada.", "INFO");
            return StatusCode(StatusCodes.Status201Created, new { Value = sucursal });
        }

        [HttpDelete]
        [Route("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var sucursal = await _dbpruebaCanguroContext.Sucursals.FindAsync(id);

            if (sucursal == null)
            {
                await RegistrarLog($"Sucursal con ID {id} no encontrada para eliminación.", "ERROR");
                return NotFound();
            }

            _dbpruebaCanguroContext.Sucursals.Remove(sucursal);
            await _dbpruebaCanguroContext.SaveChangesAsync();

            await RegistrarLog($"Sucursal con ID {id} eliminada.", "INFO");
            return StatusCode(StatusCodes.Status200OK);
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
