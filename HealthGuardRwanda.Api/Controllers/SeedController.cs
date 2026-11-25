using Microsoft.AspNetCore.Mvc;
using CoreSeedController = HealthGuardRwanda.Core.Controllers.SeedController;

namespace HealthGuardRwanda.Api.Controllers;

[ApiController]
[Route("api/seed")]
public sealed class SeedController : ApiControllerBase
{
    private readonly CoreSeedController _controller;

    public SeedController(CoreSeedController controller)
    {
        _controller = controller;
    }

    [HttpPost("load")]
    public IActionResult Load() => Respond(_controller.Load());
}
