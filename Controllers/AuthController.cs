using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XakUjin2026.DB;
using XakUjin2026.Processors;
using XakUjin2026.Model.Token;

namespace XakUjin2026.Controllers
{

    // Атрибуты, указывающие путь к контроллеру и что это контроллер API.
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Зависимости контроллера.
        private readonly ApplicationDbContext _context;
        private readonly TokenHelper _tokenHelper;
        private readonly IEmailSender _emailSender;

        // Конструктор для внедрения зависимостей.
        public AuthController(ApplicationDbContext context, TokenHelper tokenHelper, IEmailSender emailSender)
        {
            _context = context; // Контекст базы данных.
            _tokenHelper = tokenHelper; // Помощник для работы с JWT.
            _emailSender = emailSender;
        }


        // Метод для получения токена.
        [HttpPost("token")]
        //public async Task<IActionResult> GetToken([FromBody] AuthRequest request)
        public async Task<IActionResult> GetToken([FromBody] AuthRequest request) {
            // Проверка входных данных.
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Возвращает ошибку 400, если модель не валидна.
            try {
                var email = request.email;
                var password = request.password;
                // Ищем пользователя в базе по имени пользователя.
                var user = _context.Users.SingleOrDefault(u => u.Email == email);
                // Если пользователь не найден или пароль неверен, возвращаем ошибку 401.
                if (user == null || password != user.PasswordHash)
                    return Unauthorized();

                // Если email не подтвержден, возвращаем ошибку 400.
                if (!user.EmailConfirmed)
                    return BadRequest(new { Error = "Please confirm your email address first" });

                // Генерация токена и рефреш-токена.
                var token = _tokenHelper.GenerateToken(user.Username!);
                var refreshToken = Guid.NewGuid().ToString();

                // Сохранение рефреш-токена и его срока годности в базе данных.
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(60);

                _context.Update(user);
                await _context.SaveChangesAsync();

                // Возвращаем токен, рефреш-токен и имя пользователя.
                return Ok(new
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Username = user.Username,
                    Email = user.Email
                });
            }
            catch (Exception ex) {
                // В случае ошибки возвращаем ошибку 500.
                Console.WriteLine(ex);
                return StatusCode(500, new { Error = "An error occurred while processing your request. Please try again later." });
            }
        }

        // Метод для верификации пароля с помощью библиотеки BCrypt.
        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
        }

        // Метод для обновления токена с использованием рефреш-токена.
        [HttpPost("refresh-token")]
        //public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var refreshToken = request.refreshToken;
                // Получаем текущий токен из заголовка.
                string currentToken = Request.HttpContext.Request.Headers["ToInvalidToken"].ToString();
                if (!string.IsNullOrEmpty(currentToken) || !_tokenHelper.IsTokenExpired(currentToken))
                    _tokenHelper.ToInvalidToken(_tokenHelper.GetCurrentTokenId(currentToken), _tokenHelper.GetTokenExpiryDate(currentToken));

                // Ищем пользователя в базе по рефреш-токену
                var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

                // Если рефреш-токен невалидный или истек, возвращаем ошибку 401.
                if (user == null || user.RefreshTokenExpiryDate < DateTime.UtcNow)
                    return Unauthorized(new { Message = "Invalid or expired refresh token" });

                // Генерация нового токена.
                var newToken = _tokenHelper.GenerateToken(user.Username!);

                // Генерация нового рефреш-токена
                var newRefreshToken = Guid.NewGuid().ToString();
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(60);
                _context.Update(user);
                _context.SaveChanges();

                // Возвращаем новые токен и рефреш-токен.
                return Ok(new
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // В случае ошибки возвращаем ошибку 500.
                return StatusCode(500, new { Error = "An error occurred while processing your request. Please try again later." });
            }
        }

        // Метод для инвалидации текущего токена.
        [HttpPost("invalidate-token")]
        public IActionResult InvalidateCurrentToken([FromBody] InvalidateTokenRequest request)
        {
            try
            {
                var currentToken = request.currentToken;
                // Получаем текущий токен из заголовка.
                if (_tokenHelper.IsTokenExpired(currentToken))
                    return Unauthorized(new { Message = "Expired token" });

                // Инвалидация токена
                var currentTokenId = _tokenHelper.GetCurrentTokenId(currentToken);
                if (currentTokenId == null)
                    return BadRequest(new { Error = "Token ID not found" });
                var currentTokenExpiryDate = DateTime.UtcNow.AddMinutes(5); // Срок действия токена
                _tokenHelper.ToInvalidToken(currentTokenId, _tokenHelper.GetTokenExpiryDate(currentToken));
                // Возвращаем сообщение об инвалидации токена.
                return Ok(new { Message = "Token has been invalidated" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // В случае ошибки возвращаем ошибку 500.
                return StatusCode(500, new { Error = "An error occurred while processing your request. Please try again later." });
            }
        }

        // Метод действия контроллера для регистрации нового пользователя.
        [HttpPost("register")]
       public async Task<IActionResult> Register([FromBody] RegistrationRequest request) {
            var email = request.email;
            var password = request.password;
            var username = request.username;
            // Проверка на существование пользователя с таким же адресом электронной почты.
            var existingEmail= await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            var existingUsername = await _context.Users
                .FirstOrDefaultAsync (u => u.Username == username);

            if (existingEmail != null || existingUsername != null)
            {
                return BadRequest(new { Error = "Email or Username is already taken" });
            }

            // Хеширование пароля
            string? hashedPassword = password;
            string emailConfirmationCode = Guid.NewGuid().ToString();

            // Создание нового пользователя
            var newUser = new ApplicationUserEntity
            {
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                EmailConfirmed = true,
                EmailConfirmationCode = emailConfirmationCode
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Создание ссылки для подтверждения адреса электронной почты.
            //var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/v1/auth/confirm-email?code={emailConfirmationCode}";
            //if (email != null && confirmationLink != null)
                //await _emailSender.EmailConfirmationMessage(email!, confirmationLink!);

            return Ok(new { Message = "Registration successful" });
        }

        // Метод действия контроллера для подтверждения адреса электронной почты.
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string code)
        {
            // Проверка на существование пользователя с таким же именем пользователя.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailConfirmationCode == code);
            if (user == null)
                return NotFound("Confirmation code is invalid.");

            // Подтверждение адреса электронной почты и очистка кода подтверждения.
            user.EmailConfirmed = true;
            user.EmailConfirmationCode = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Email confirmed successfully" });
        }
    }
}
