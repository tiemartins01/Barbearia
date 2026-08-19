using BarbeariaCore.Application.Abstractions;
using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Security.Claims;

namespace BarbeariaApi.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IRefreshTokenService _refresh;
        private readonly ICurrentUser _user;
        private const string AccessCookie = "access-token";
        private const string RefreshCookie = "refresh-token";
        private static readonly TimeSpan AccessExpiration =
    TimeSpan.FromMinutes(15);
        private static readonly TimeSpan RefreshExpiration =
            TimeSpan.FromDays(7);
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILoginService loginService, IRefreshTokenService refresh, ICurrentUser user, ILogger<LoginController> logger)
        {
            _loginService = loginService;
            _refresh = refresh;
            _user = user;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("me")]
        [HttpGet("~/api/v1/auth/me")]
        public IActionResult Me()
        {
            return Ok(new DTOResponseMe
            {
                Id = _user.UserId,
                Nome = _user.Name,
                Role = _user.Role
            });
        }

        // ################################## TESTE ##################################
        //[HttpGet("teste-cookie")]
        //public IActionResult TesteCookie()
        //{
        //    Response.Cookies.Append(
        //        "teste",
        //        "123",
        //        new CookieOptions
        //        {
        //            HttpOnly = false,
        //            Secure = false,
        //            SameSite = SameSiteMode.Lax
        //        });

        //    return Ok(new
        //    {
        //        Cookie = Response.Headers["Set-Cookie"].ToString()
        //    });
        //}


        [HttpPost]
        [HttpPost("~/api/v1/auth/login")]
        [EnableRateLimiting("login")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Login([FromBody] DTOLoginUsuario request)
        {
            var resultado = await _loginService.RealizarLoginAsync(request.Nome, request.Senha);

            SalvarCookies(resultado);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        [HttpPost("~/api/v1/auth/logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var refresh = Request.Cookies[RefreshCookie];

            if (!string.IsNullOrWhiteSpace(refresh))
            {
                try
                {
                    await _refresh.RevokeTokenAsync(refresh);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
      ex,
      "Não foi possível revogar o refresh token durante o logout.");
                }
            }

            Response.Cookies.Delete(RefreshCookie, CookieOptionsRefresh());
            Response.Cookies.Delete(AccessCookie, CookieOptionsAccess());

            return NoContent();
        }


        [HttpPost("refresh")]
        [HttpPost("~/api/v1/auth/refresh")]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies[RefreshCookie];

            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized();

            var resultado = await _refresh.GerarRefreshAsync(refreshToken);

            //Response.Cookies.Append("teste", "123");

            SalvarCookies(resultado);

            return NoContent();
        }

        [Authorize]
        [HttpGet("sessions")]
        [HttpGet("~/api/v1/auth/sessions")]
        public async Task<IActionResult> Sessions()
        {
            var current = Request.Cookies[RefreshCookie];
            var sessions = await _refresh.ListarSessoesAsync(_user.UserId, current);
            return Ok(sessions);
        }

        [Authorize]
        [HttpDelete("sessions")]
        [HttpDelete("~/api/v1/auth/sessions")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeAllSessions()
        {
            await _refresh.RevogarTodasAsync(_user.UserId);
            Response.Cookies.Delete(RefreshCookie, CookieOptionsRefresh());
            Response.Cookies.Delete(AccessCookie, CookieOptionsAccess());
            return NoContent();
        }


        [Authorize]
        [HttpDelete("sessions/{sessionId:int}")]
        [HttpDelete("~/api/v1/auth/sessions/{sessionId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeSession([FromRoute] int sessionId)
        {
            await _refresh.RevogarSessaoAsync(_user.UserId, sessionId);
            return NoContent();
        }

        private CookieOptions CookieOptionsAccess()
        {
            var isHttps = Request.IsHttps;

            return new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = isHttps
            ? SameSiteMode.None
            : SameSiteMode.Lax,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.Add(AccessExpiration),
                Path = "/"
            };
        }

        private CookieOptions CookieOptionsRefresh()
        {
            var isHttps = Request.IsHttps;

            return new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = isHttps
            ? SameSiteMode.None
            : SameSiteMode.Lax,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.Add(RefreshExpiration),
                Path = "/"
            };
        }

        private void SalvarCookies(DTOAuthResponse dto)
        {
            Response.Cookies.Append(
                AccessCookie,
                dto.accessToken,
                CookieOptionsAccess());

            Response.Cookies.Append(
                RefreshCookie,
                dto.refreshToken,
                CookieOptionsRefresh());
        }
    }
}
