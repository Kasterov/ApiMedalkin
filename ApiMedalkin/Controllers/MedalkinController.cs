using ApiMedalkin.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace ApiMedalkin.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedalkinController : ControllerBase
{
    private readonly IUpdateService _updateService;

    public MedalkinController(IUpdateService updateService)
    {
        _updateService = updateService;
    }

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] Update update,
        CancellationToken cancellationToken = default)
    {
        await _updateService.HandleUpdate(update, cancellationToken);
        return Ok("Ok!");
    }

    [HttpGet]
    [Route("health")]
    public async Task<IActionResult> Get()
    {
        return Ok("Ok! Server is listening!");
    }
}
