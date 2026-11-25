using HealthGuardRwanda.Core.Models;
using Microsoft.AspNetCore.Mvc;
using CoreControllers = HealthGuardRwanda.Core.Controllers;

namespace HealthGuardRwanda.Api.Controllers;

[ApiController]
[Route("api/coverage-verifications")]
public sealed class CoverageVerificationsController : ApiControllerBase
{
    private readonly CoreControllers.CoverageVerificationController _controller;

    public CoverageVerificationsController(CoreControllers.CoverageVerificationController controller)
    {
        _controller = controller;
    }

    [HttpPost("save")]
    public IActionResult Save([FromQuery] Guid enrollmentId, [FromBody] CoverageVerification entity)
    {
        entity.EnrollmentId = enrollmentId;
        entity.Enrollment = null!;
        return Respond(_controller.Save(entity));
    }

    [HttpGet("citizen/{nationalId}")]
    public IActionResult ByCitizen(string nationalId) => Respond(_controller.ByCitizen(nationalId));

    [HttpDelete("delete/{id:guid}")]
    public IActionResult Delete(Guid id) => Respond(_controller.Delete(id));
}
