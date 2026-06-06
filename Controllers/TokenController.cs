using Microsoft.AspNetCore.Mvc;
using XakUjin2026.Processors;
using Microsoft.EntityFrameworkCore;

namespace XakUjin2026.Controllers
{
    public class TokenController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly TokenHelper _tokenHelper;

        public TokenController(ApplicationDbContext context, TokenHelper tokenHelper)
        {
            _context = context;
            _tokenHelper = tokenHelper;

        }
        public async Task<IActionResult?> EnsureAuthorizedAsync(string? authToken)
        {
            // Токен не прислан — пропускаем как фейкового пользователя (временная заглушка).
            if (string.IsNullOrWhiteSpace(authToken))
            {
                var fakeUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == Const.FakeUserUsername);
                return fakeUser == null
                    ? NotFound(new { Message = "Fake user not found. Restart the app to seed it." })
                    : null;
            }

            if (_tokenHelper.IsTokenExpired(authToken))
                return Unauthorized(new { Message = "Expired token" });

            var currentTokenId = _tokenHelper.GetCurrentTokenId(authToken);
            if (_tokenHelper.IsInvalidToken(currentTokenId))
                return Unauthorized(new { Message = "This token has been invalidated." });

            var username = _tokenHelper.ExtractUsernameFromToken(authToken);
            if (username == null)
                return Unauthorized(new { Message = "Invalid token" });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return Unauthorized(new { Message = "Invalid token" });

            return null; // авторизован
        }
    }
}