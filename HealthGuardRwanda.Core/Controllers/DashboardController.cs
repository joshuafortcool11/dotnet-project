using HealthGuardRwanda.Core.Services;

namespace HealthGuardRwanda.Core.Controllers;

public class DashboardController : ControllerBase
{
    private readonly DashboardService _service;

    public DashboardController(DashboardService service)
    {
        _service = service;
    }

    public ServiceResult Summary() => Ok(_service.Summary());
}

public class SeedController : ControllerBase
{
    private readonly SeedService _service;

    public SeedController(SeedService service)
    {
        _service = service;
    }

    public ServiceResult Load() => Response(200, _service.Seed());
}
