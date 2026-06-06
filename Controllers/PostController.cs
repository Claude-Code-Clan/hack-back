using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XakUjin2026.DB;
using XakUjin2026.Model.InternalRequest;
using XakUjin2026.Processors;


namespace XakUjin2026.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenHelper _tokenHelper;
        private readonly TokenController _tokenController;

        public PostController(ApplicationDbContext context, TokenHelper tokenHelper)
        {
            _context = context;
            _tokenHelper = tokenHelper;
            _tokenController = new TokenController(_context, _tokenHelper);

        }

        [HttpPost("display-set")]
        public async Task<IActionResult> PostDisplaySet([FromBody] Model.InternalRequest.DisplaySetRequest request, [FromHeader(Name = "Authorization")] string authorizationHeader = null)
        {
            try
            {
                var authError = await _tokenController.EnsureAuthorizedAsync(authorizationHeader);
                if (authError != null)
                    return authError;

                var displayIds = (request.DisplayIds ?? Array.Empty<int?>())
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .Distinct()
                    .ToList();

                var widgetRequests = (request.Widgets ?? new List<WidgetDataRequest>())
                    .Where(w => !string.IsNullOrWhiteSpace(w.Type))
                    .ToList();

                var titles = widgetRequests.Select(w => w.Type!).Distinct().ToList();
                var typeByTitle = (await _context.WidgetTypes
                        .Where(t => titles.Contains(t.Title!))
                        .ToListAsync())
                    .ToDictionary(t => t.Title!, t => t);

                // Неизвестный тип виджета — ошибка 400.
                var unknownTypes = titles.Where(t => !typeByTitle.ContainsKey(t)).ToList();
                if (unknownTypes.Count > 0)
                    return BadRequest(new { Message = "Unknown widget type(s)", types = unknownTypes });

                var devices = await _context.Devices
                    .Include(d => d.Widgets)
                    .Where(d => displayIds.Contains(d.Id))
                    .ToListAsync();

                foreach (var device in devices)
                {
                    foreach (var wr in widgetRequests)
                    {
                        var type = typeByTitle[wr.Type!];
                        var widget = device.Widgets.FirstOrDefault(w => w.WidgetTypeId == type.Id);
                        if (widget == null)
                        {
                            widget = new Widget { DeviceId = device.Id, WidgetTypeId = type.Id };
                            device.Widgets.Add(widget);
                        }

                        widget.XPosition = wr.x;
                        widget.YPosition = wr.y;
                        widget.Width = wr.w;
                        widget.Height = wr.h;
                    }
                }

                await _context.SaveChangesAsync();

                await _context.Widgets
                    .Where(w => !_context.Devices.Any(d => d.Id == w.DeviceId))
                    .ExecuteDeleteAsync();

                var result = await _context.Devices
                    .Where(d => displayIds.Contains(d.Id))
                    .Select(d => new
                    {
                        id = d.Id,
                        deviceType = d.DeviceType!.Name,
                        entranceId = d.EntranceId,
                        widgets = d.Widgets.Select(wg => new
                        {
                            id = wg.Id,
                            widgetType = wg.WidgetType!.Title,
                            x = wg.XPosition,
                            y = wg.YPosition,
                            w = wg.Width,
                            h = wg.Height
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(new { devices = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing the request: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}