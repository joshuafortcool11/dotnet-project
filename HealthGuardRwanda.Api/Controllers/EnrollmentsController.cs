using HealthGuardRwanda.Core.Models;
using Microsoft.AspNetCore.Mvc;
using CoreControllers = HealthGuardRwanda.Core.Controllers;

namespace HealthGuardRwanda.Api.Controllers;

[ApiController]
[Route("api/enrollments")]
public sealed class EnrollmentsController : ApiControllerBase
{
    private readonly CoreControllers.EnrollmentController _controller;

    public EnrollmentsController(CoreControllers.EnrollmentController controller)
    {
        _controller = controller;
    }

    [HttpPost("save")]
    public IActionResult Save([FromBody] Enrollment entity) => Respond(_controller.Save(entity));

    [HttpGet("all")]
    public IActionResult All() => Respond(_controller.GetAll());

    [HttpGet("by-status")]
    public IActionResult ByStatus([FromQuery] EnrollmentStatus status) => Respond(_controller.ByStatus(status));

    [HttpGet("between")]
    public IActionResult Between([FromQuery] string start, [FromQuery] string end) =>
        Respond(_controller.Between(start, end));

    [HttpGet("stats/by-status")]
    public IActionResult StatsByStatus() => Respond(_controller.StatsByStatus());

    [HttpGet("page")]
    public IActionResult Page([FromQuery] int page = 0, [FromQuery] int size = 5,
        [FromQuery] string sort = "enrollmentDate,desc") =>
        Respond(_controller.Page(Math.Max(0, page), Math.Max(1, size), sort));

    [HttpPatch("{id:guid}/status")]
    public IActionResult UpdateStatus(Guid id, [FromQuery] EnrollmentStatus status) =>
        Respond(_controller.UpdateStatus(id, status));

    [HttpPatch("cancel-day")]
    public IActionResult CancelDay([FromQuery] Guid enrollmentOfficerId, [FromQuery] string date) =>
        Respond(_controller.CancelDay(enrollmentOfficerId, date));

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id) => Respond(_controller.GetById(id));

    [HttpPut("update/{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] Enrollment entity) => Respond(_controller.Update(id, entity));

    [HttpDelete("delete/{id:guid}")]
    public IActionResult Delete(Guid id) => Respond(_controller.Delete(id));
}
