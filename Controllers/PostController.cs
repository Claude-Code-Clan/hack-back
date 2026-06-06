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

                var deviceIds = await _context.Devices
                    .Where(d => displayIds.Contains(d.Id))
                    .Select(d => d.Id)
                    .ToListAsync();

                await _context.Widgets
                    .Where(w => deviceIds.Contains(w.DeviceId))
                    .ExecuteDeleteAsync();

                var newWidgets = deviceIds
                    .SelectMany(deviceId => widgetRequests.Select(wr => new WidgetEntity
                    {
                        DeviceId = deviceId,
                        WidgetTypeId = typeByTitle[wr.Type!].Id,
                        XPosition = wr.x,
                        YPosition = wr.y,
                        Width = wr.w,
                        Height = wr.h
                    }))
                    .ToList();

                if (newWidgets.Count > 0)
                {
                    _context.Widgets.AddRange(newWidgets);
                    await _context.SaveChangesAsync();
                }

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
        [HttpPost("rss-link-add")]
        public async Task<IActionResult> PostRSSLinkAdd([FromBody] RssLinksRequest request, [FromHeader(Name = "Authorization")] string authorizationHeader = null)
        {
            try
            {
                var authError = await _tokenController.EnsureAuthorizedAsync(authorizationHeader);
                if (authError != null)
                    return authError;

                var incoming = (request?.rssLinks ?? new List<RssLink>())
                    .Where(r => !string.IsNullOrWhiteSpace(r.Title))
                    .GroupBy(r => r.Title!.Trim())
                    .Select(g => g.First())
                    .ToList();

                var titles = incoming.Select(r => r.Title!.Trim()).ToList();

                var existingTitles = (await _context.RSSLinks
                        .Where(r => titles.Contains(r.Title!))
                        .Select(r => r.Title!)
                        .ToListAsync())
                    .ToHashSet();

                var newLinks = incoming
                    .Where(r => !existingTitles.Contains(r.Title!.Trim()))
                    .Select(r => new RSSLinksEntity
                    {
                        Title = r.Title!.Trim(),
                        Link = r.Link?.Trim()
                    })
                    .ToList();

                if (newLinks.Count > 0)
                {
                    _context.RSSLinks.AddRange(newLinks);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        return Conflict(new
                        {
                            Message = "One or more RSS link titles already exist",
                            titles = newLinks.Select(l => l.Title)
                        });
                    }
                }

                return Ok(new { added = newLinks.Select(l => new { l.Id, l.Title, l.Link }) });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing the request: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPost("rss-link-edit")]
        public async Task<IActionResult> PostRSSLinkEdit([FromBody] RssLinksRequest request, [FromHeader(Name = "Authorization")] string authorizationHeader = null)
        {
            try
            {
                var authError = await _tokenController.EnsureAuthorizedAsync(authorizationHeader);
                if (authError != null)
                    return authError;

                var incoming = (request?.rssLinks ?? new List<RssLink>())
                    .Where(r => r.Id.HasValue & !string.IsNullOrWhiteSpace(r.Link) & !string.IsNullOrWhiteSpace(r.Title))
                    .GroupBy(r => r.Id)
                    .Select(g => g.First())
                    .ToList();
                
                var ids = incoming.Select(r => r.Id!.Value).ToList();

                // Существующие записи по присланным id.
                var existing = await _context.RSSLinks
                    .Where(r => ids.Contains(r.Id))
                    .ToDictionaryAsync(r => r.Id);

                var updated = new List<RSSLinksEntity>();
                var notFound = new List<int>();

                foreach (var rss in incoming)
                {
                    if (existing.TryGetValue(rss.Id!.Value, out var entity))
                    {
                        entity.Title = rss.Title!.Trim();
                        entity.Link = rss.Link!.Trim();
                        updated.Add(entity);
                    }
                    else
                    {
                        notFound.Add(rss.Id!.Value);
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    return Conflict(new
                    {
                        Message = "RSS link title must be unique",
                        titles = updated.Select(u => u.Title)
                    });
                }

                return Ok(new
                {
                    updated = updated.Select(u => new { u.Id, u.Title, u.Link }),
                    notFound
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing the request: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

    }
}