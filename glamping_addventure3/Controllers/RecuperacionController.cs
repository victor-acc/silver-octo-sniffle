//using glamping_addventure3.Models;
//using Glamping_Addventure3.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace Glamping_Addventure2.Controllers
//{
//    [AllowAnonymous]

//    [Route("Recuperacion/[action]")]
//    public class RecuperacionController : Controller
//    {
//        private readonly GlampingAddventure3Context _context;
//        private readonly IEmailService _emailService;

//        public RecuperacionController(GlampingAddventure3Context context, IEmailService emailService)
//        {
//            _context = context;
//            _emailService = emailService;
//        }

//        // Acción para mostrar la vista de solicitar recuperación de contraseña
//        [HttpGet("SolicitarRecuperacion")]
//        public IActionResult SolicitarRecuperacion()
//        {
//            return View();
//        }

//        [HttpPost("enviar-recuperacion")]
//        public async Task<IActionResult> EnviarRecuperacion([FromForm] string email)
//        {
//            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
//            {
//                return BadRequest("El correo electrónico no tiene un formato válido.");
//            }

//            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
//            if (usuario == null)
//            {
//                return BadRequest("El correo electrónico no está registrado.");
//            }

//            // Generar un código de recuperación aleatorio
//            var codigo = new Random().Next(100000, 999999).ToString();

//            // Guardar el código de recuperación en la base de datos con su fecha de expiración
//            var codigoRecuperacion = new CodigoRecuperacion
//            {
//                UsuarioId = usuario.Idusuario,
//                Codigo = codigo,
//                FechaExpiracion = DateTime.Now.AddMinutes(15), // El código expira en 15 minutos
//                Usado = false
//            };

//            _context.CodigosRecuperacion.Add(codigoRecuperacion);
//            await _context.SaveChangesAsync();

//            // Crear el mensaje con el código
//            var subject = "Recuperación de Contraseña";
//            var message = $@"
//        <html>
//        <body>
//            <p>Tu código de recuperación es: <strong>{codigo}</strong></p>
//            <p>Este código expirará en 15 minutos.</p>
//        </body>
//        </html>";

//            // Enviar el correo
//            await _emailService.EnviarCorreoRecuperacion(usuario.Email, subject, message);

//            return Ok("Se ha enviado un código de recuperación a tu correo electrónico.");
//        }

//        // Cambiar contraseña usando el token
//        [HttpPost("cambiar")]
//        public async Task<IActionResult> CambiarContrasena([FromForm] CambioContrasenaDto cambioContrasena)
//        {
//            // Validar que el token sea válido
//            var tokenRecuperacion = await _context.TokenRecuperacion
//                .FirstOrDefaultAsync(t => t.Token == cambioContrasena.Token && !t.Usado && t.FechaExpiracion > DateTime.Now);

//            if (tokenRecuperacion == null)
//                return BadRequest("Token inválido o expirado");

//            // Obtener al usuario asociado al token
//            var usuario = await _context.Usuarios.FindAsync(tokenRecuperacion.UsuarioId);
//            if (usuario == null)
//                return NotFound("Usuario no encontrado");

//            // Validar la nueva contraseña
//            if (string.IsNullOrWhiteSpace(cambioContrasena.NuevaContrasena) || cambioContrasena.NuevaContrasena.Length < 6)
//            {
//                return BadRequest("La contraseña debe tener al menos 6 caracteres.");
//            }

//            // Actualizar la contraseña y marcar el token como usado
//            usuario.Contrasena = cambioContrasena.NuevaContrasena;
//            tokenRecuperacion.Usado = true;

//            await _context.SaveChangesAsync();
//            return Ok("Contraseña actualizada exitosamente");
//        }
//    }

//    // DTO para cambio de contraseña
//    public class CambioContrasenaDto
//    {
//        public string Token { get; set; }
//        public string NuevaContrasena { get; set; }
//    }
//}