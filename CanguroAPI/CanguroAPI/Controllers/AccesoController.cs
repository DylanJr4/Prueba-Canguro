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
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly DbpruebaCanguroContext _dbpruebaCanguroContext;
        private readonly Utilidades _utilidades;
        public AccesoController(DbpruebaCanguroContext dbpruebaCanguroContext, Utilidades utilidades)
        {
            _dbpruebaCanguroContext = dbpruebaCanguroContext;
            _utilidades = utilidades;
        }

        [HttpPost]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse(UsuarioDTO obj)
        {
            var modeloUsuario = new Usuario
            {
                Nombre = obj.Nombre,
                Correo = obj.Correo,
                Clave = _utilidades.encriptarSHA256(obj.Clave)
            };
            await _dbpruebaCanguroContext.Usuarios.AddAsync(modeloUsuario);
            await _dbpruebaCanguroContext.SaveChangesAsync();

            if (modeloUsuario.IdUsuario != 0)
            {
                await RegistrarLog("Usuario registrado exitosamente", "INFO", modeloUsuario.IdUsuario);
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            }
            else
            {
                await RegistrarLog("Error al registrar usuario", "ERROR");
                return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false });
            }


        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO obj)
        {
            var usuarioEncontrado = await _dbpruebaCanguroContext.Usuarios
                .Where(u =>
                u.Correo == obj.Correo && u.Clave == _utilidades.encriptarSHA256(obj.Clave)
                ).FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
            {
                await RegistrarLog("Intento de inicio de sesión fallido", "WARN");
                return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false, token = "" });
            }
            else
            {
                await RegistrarLog("Inicio de sesión exitoso", "INFO", usuarioEncontrado.IdUsuario);
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utilidades.generarJWT(usuarioEncontrado) });
            }
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
