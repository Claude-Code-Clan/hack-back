using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace XakUjin2026.Controllers
{
    // Определение маршрута и атрибута контроллера для данного класса контроллера.
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GetController : Controller
    {
        // Объявляются частные поля для контекста базы данных и вспомогательного класса для работы с токенами.
        private readonly ApplicationDbContext _context;
        private readonly TokenHelper _tokenHelper;

        // Конструктор контроллера с внедрением зависимостей для контекста базы данных и вспомогательного класса токенов.
        public GetController(ApplicationDbContext context, TokenHelper tokenHelper)
        {
            _context = context;
            _tokenHelper = tokenHelper;
        }

        // Метод действия контроллера для получения имени пользователя из токена.
        [HttpPost("username-from-token")]
        public async Task<IActionResult> GetUserNameFromToken([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                // Извлечение имени пользователя из токена.
                if (_tokenHelper.IsTokenExpired(authorizationHeader))
                    return Unauthorized(new { Message = "Expired token" });

                // Получение идентификатора текущего токена.
                var currentTokenId = _tokenHelper.GetCurrentTokenId(authorizationHeader);

                // Проверка действительности токена.
                if (_tokenHelper.IsInvalidToken(currentTokenId))
                    return Unauthorized(new { Message = "This token has been invalidated." });

                // Извлечение имени пользователя из токена.
                var username = _tokenHelper.ExtractUsernameFromToken(Request.Headers["Authorization"].ToString());


                // Если имя пользователя найдено, возвращается ответ с именем пользователя.
                if (username != null)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                    return Ok(new { user });
                }
                // В противном случае возвращается ответ об ошибке.
                else
                    return Unauthorized(new { Message = "Invalid token" });
            }
            // Обработка исключений и возврат ошибки сервера.
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { Error = "An error occurred while processing your request. Please try again later." });
            }
        }

        //Метод для получения Ujin токена из базы данных по JWT токену.
        [HttpPost("ujin-token")]
        public async Task<IActionResult> GetUjinToken([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authorizationHeader))
                {
                    var fakeUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == Const.FakeUserUsername);
                    if (fakeUser == null)
                        return NotFound(new { Message = "Fake user not found. Restart the app to seed it." });
                    return Ok(new { ujinToken = fakeUser.UjinToken });
                }

                // Извлечение имени пользователя из токена.
                if (_tokenHelper.IsTokenExpired(authorizationHeader))
                    return Unauthorized(new { Message = "Expired token" });

                // Получение идентификатора текущего токена.
                var currentTokenId = _tokenHelper.GetCurrentTokenId(authorizationHeader);

                // Проверка действительности токена.
                if (_tokenHelper.IsInvalidToken(currentTokenId))
                    return Unauthorized(new { Message = "This token has been invalidated." });

                // Извлечение имени пользователя из токена.
                var username = _tokenHelper.ExtractUsernameFromToken(Request.Headers["Authorization"].ToString());

                // Если имя пользователя найдено, возвращается ответ с Ujin токеном.
                if (username != null)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                    return Ok(new { ujinToken = user?.UjinToken });
                }
                // В противном случае возвращается ответ об ошибке.
                else
                    return Unauthorized(new { Message = "Invalid token" });
            }
            // Обработка исключений и возврат ошибки сервера.
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { Error = "An error occurred while processing your request. Please try again later." });
            }
        }

        [HttpGet("complex-list")]
        public async Task<IActionResult> GetComplexList([FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            try
            {
                var ujinTokenResult = await GetUjinToken(authorizationHeader) as ObjectResult;
                if (ujinTokenResult == null || ujinTokenResult.Value == null)
                    return Unauthorized(new { Message = "Unable to retrieve Ujin token" });

                var ujinToken = (ujinTokenResult.Value as dynamic).ujinToken;
                if (ujinToken == null)
                    return Unauthorized(new { Message = "Ujin token not found" });

                var complexListRequest = new ComplexListExtRequest(ujinToken);
                var complexListResponse = await complexListRequest.SendAsync();

                if (complexListResponse != null)
                    return Ok(complexListResponse);
                else
                    return StatusCode(502, new { Message = "Failed to retrieve complex list from external API" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { Error = "An error occurred while processing your request. Please try again later." });
            }
        }
    }
}
