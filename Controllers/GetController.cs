using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("ujin-token")]
        public async Task<IActionResult> GetUjinToken([FromHeader(Name = "Authorization")] string? authorizationHeader = null)
        {
            try
            {
                var (ujinToken, error) = await ResolveUjinTokenAsync(authorizationHeader);
                if (error != null)
                    return error;

                return Ok(new { ujinToken });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { Error = "An error occurred while processing your request. Please try again later." });
            }
        }

        [HttpGet("complex-list")]
        public async Task<IActionResult> GetComplexList([FromHeader(Name = "Authorization")] string? authorizationHeader = null)
        {
            try
            {
                var (ujinToken, error) = await ResolveUjinTokenAsync(authorizationHeader);
                if (error != null)
                    return error;
                if (string.IsNullOrEmpty(ujinToken))
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

        [HttpGet("building-list")]
        public async Task<IActionResult> GetBuildingList([FromHeader(Name = "Authorization")] string? authorizationHeader = null)
        {
            try
            {
                var (ujinToken, error) = await ResolveUjinTokenAsync(authorizationHeader);
                if (error != null)
                    return error;
                if (string.IsNullOrEmpty(ujinToken))
                    return Unauthorized(new { Message = "Ujin token not found" });

                var buildingListRequest = new BuildingListExtRequest(ujinToken);
                var buildingListResponse = await buildingListRequest.SendAsync();

                if (buildingListResponse == null)
                    return StatusCode(502, new { Message = "Failed to retrieve building list from external API" });

                await BuildingSyncService.SyncAsync(_context, buildingListResponse);

                var buildings = await _context.Buildings
                    .Include(b => b.Entrances)
                        .ThenInclude(e => e.Devices)
                            .ThenInclude(d => d.DeviceType)
                    .ToListAsync();

                var result = buildings.Select(b => new
                {
                    id = b.Id,
                    title = b.Title,
                    entrances = b.Entrances.Select(e => new EntranceResponse
                    {
                        id = e.Id,
                        buildingId = b.Id,
                        entranceNumber = e.Number?.ToString(),
                        devices = e.Devices.Select(d => new DeviceResponse
                        {
                            id = d.Id,
                            deviceType = d.DeviceType?.Name
                        }).ToList()
                    }).ToList()
                }).ToList();

                return Ok(new { buildings = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { Error = "An error occurred while processing your request. Please try again later." });
            }
        }

        [HttpGet("devices-list")]
        public async Task<IActionResult> GetDevicesList([FromQuery] int entranceId, [FromHeader(Name = "Authorization")] string? authorizationHeader = null)
        {
            try
            {
                var (ujinToken, error) = await ResolveUjinTokenAsync(authorizationHeader);
                if (error != null)
                    return error;
                if (string.IsNullOrEmpty(ujinToken))
                    return Unauthorized(new { Message = "Ujin token not found" });

                var entrance = await _context.Entrances
                    .Include(e => e.Devices)
                        .ThenInclude(d => d.DeviceType)
                    .FirstOrDefaultAsync(e => e.Id == entranceId);

                if (entrance == null)
                    return NotFound(new { Message = "Entrance not found" });

                var devices = entrance.Devices
                    .Select(d => new DeviceResponse
                    {
                        id = d.Id,
                        deviceType = d.DeviceType?.Name
                    })
                    .ToList();

                return Ok(new { entranceId, devices });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { Error = "An error occurred while processing your request. Please try again later." });
            }
        }
        private async Task<(string? token, IActionResult? error)> ResolveUjinTokenAsync(string? authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
            {
                var fakeUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == Const.FakeUserUsername);
                if (fakeUser == null)
                    return (null, NotFound(new { Message = "Fake user not found. Restart the app to seed it." }));
                return (fakeUser.UjinToken, null);
            }

            if (_tokenHelper.IsTokenExpired(authToken))
                return (null, Unauthorized(new { Message = "Expired token" }));

            var currentTokenId = _tokenHelper.GetCurrentTokenId(authToken);
            if (_tokenHelper.IsInvalidToken(currentTokenId))
                return (null, Unauthorized(new { Message = "This token has been invalidated." }));

            var username = _tokenHelper.ExtractUsernameFromToken(authToken);
            if (username == null)
                return (null, Unauthorized(new { Message = "Invalid token" }));

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return (user?.UjinToken, null);
        }
    }
}
