using LoginBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuentasController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext dbContext;

        public CuentasController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.dbContext = dbContext;
        }
        //Final
        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuario.Email,
                Email = credencialesUsuario.Email,
            };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            return BadRequest(resultado.Errors);
        }

        private async Task<ActionResult<RespuestaAutenticacion>> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
        {
            new Claim("email",credencialesUsuario.Email)
        };
            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsRoles = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claims);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["LlaveJWT"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddDays(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion
            {
                token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                expiracion = expiracion,
            };
        }

        [HttpGet("RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
        {

            var emailClaims = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Email).Select(x => x.Value).FirstOrDefault();
            var credencialesUsuario = new CredencialesUsuario() { Email = emailClaims };

            return await ConstruirToken(credencialesUsuario);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(
                credencialesUsuario.Email,
                credencialesUsuario.Password,
                isPersistent: false,
                lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }
        }


        [HttpPost("MonsterFavorito")]
        public ActionResult AgregarMonsterFavorito([FromBody] MonsterFavorite monster)
        {
            var String num = "";
            if (monster == null)
            {
                return BadRequest("Datos inválidos");
            }

            // Puedes realizar validaciones adicionales aquí
            var monsterExistente = dbContext.MonstersFavoritos
      .FirstOrDefault(j => j.UserId == monster.UserId && j.MonsterName == monster.MonsterName);

            if (monsterExistente != null)
            {
                return BadRequest("El moustro ya está en la lista de favoritos");
            }

            dbContext.MonstersFavoritos.Add(monster);
            dbContext.SaveChanges();

            return Ok();

        }



        [HttpDelete("EliminarMonsterFavorito")]
        public ActionResult EliminarMonsterFavorito([FromBody] MonsterFavorite monster)
        {
            if (monster == null)
            {
                return BadRequest("Datos inválidos");
            }

            // Puedes realizar validaciones adicionales aquí
            var monsterExistente = dbContext.MonstersFavoritos
     .FirstOrDefault(j => j.UserId == monster.UserId && j.MonsterName == monster.MonsterName);

            if (monsterExistente == null)
            {
                return NotFound("El moustro no se encuentra en la lista de favoritos");
            }


            dbContext.MonstersFavoritos.Remove(monsterExistente);
            dbContext.SaveChanges();

            return Ok();

        }



        [HttpGet("{userId}")]
        public IActionResult ObtenerMonstersFavoritos(string userId)
        {
            var monstersFavoritos = dbContext.MonstersFavoritos
                .Where(j => j.UserId == userId)
                .ToList();

            return Ok(monstersFavoritos);
        }

    }

}

