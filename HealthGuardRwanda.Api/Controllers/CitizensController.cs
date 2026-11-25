using HealthGuardRwanda.Core.Models;
using Microsoft.AspNetCore.Mvc;
using CoreControllers = HealthGuardRwanda.Core.Controllers;

namespace HealthGuardRwanda.Api.Controllers;

[ApiController]
[Route("api/citizens")]
public sealed class CitizensController : ApiControllerBase
{
    private readonly CoreControllers.CitizenController _controller;

    public CitizensController(CoreControllers.CitizenController controller)
    {
        _controller = controller;
    }

    [HttpPost("save")]
    public IActionResult Save([FromBody] Citizen entity) => Respond(_controller.Save(entity));

    [HttpGet("all")]
    public IActionResult All() => Respond(_controller.GetAll());

    [HttpGet("by-village")]
    public IActionResult ByVillage([FromQuery] string village) => Respond(_controller.ByVillage(village));

    [HttpGet("of-enrollment-officer/{enrollmentOfficerId:guid}")]
    public IActionResult OfEnrollmentOfficer(Guid enrollmentOfficerId) =>
        Respond(_controller.OfEnrollmentOfficer(enrollmentOfficerId));

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id) => Respond(_controller.GetById(id));

    [HttpPut("update/{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] Citizen entity) => Respond(_controller.Update(id, entity));

    [HttpDelete("delete/{id:guid}")]
    public IActionResult Delete(Guid id) => Respond(_controller.Delete(id));
}
