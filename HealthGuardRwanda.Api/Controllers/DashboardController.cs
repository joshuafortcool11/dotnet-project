using Microsoft.AspNetCore.Mvc;
using CoreDashboardController = HealthGuardRwanda.Core.Controllers.DashboardController;

namespace HealthGuardRwanda.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ApiControllerBase
{
    private readonly CoreDashboardController _controller;

    public DashboardController(CoreDashboardController controller)
    {
        _controller = controller;
    }

    [HttpGet("summary")]
    public IActionResult Summary() => Respond(_controller.Summary());
}
