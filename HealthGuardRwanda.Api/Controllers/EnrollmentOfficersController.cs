using HealthGuardRwanda.Core.Models;
using Microsoft.AspNetCore.Mvc;
using CoreControllers = HealthGuardRwanda.Core.Controllers;

namespace HealthGuardRwanda.Api.Controllers;

[ApiController]
[Route("api/enrollment-officers")]
public sealed class EnrollmentOfficersController : ApiControllerBase
{
    private readonly CoreControllers.EnrollmentOfficerController _controller;

    public EnrollmentOfficersController(CoreControllers.EnrollmentOfficerController controller)
    {
        _controller = controller;
    }

    [HttpPost("save")]
    public IActionResult Save([FromBody] EnrollmentOfficer entity) => Respond(_controller.Save(entity));

    [HttpGet("all")]
    public IActionResult All() => Respond(_controller.GetAll());

    [HttpGet("by-household-category")]
    public IActionResult ByHouseholdCategory([FromQuery] string name) => Respond(_controller.ByHouseholdCategory(name));

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id) => Respond(_controller.GetById(id));

    [HttpPut("update/{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] EnrollmentOfficer entity) => Respond(_controller.Update(id, entity));

    [HttpDelete("delete/{id:guid}")]
    public IActionResult Delete(Guid id) => Respond(_controller.Delete(id));

    [HttpPut("{enrollmentOfficerId:guid}/household-categories/{householdCategoryId:guid}")]
    public IActionResult AddHouseholdCategory(Guid enrollmentOfficerId, Guid householdCategoryId) =>
        Respond(_controller.AddHouseholdCategory(enrollmentOfficerId, householdCategoryId));

    [HttpPatch("{enrollmentOfficerId:guid}/sector")]
    public IActionResult MoveToSector(Guid enrollmentOfficerId, [FromQuery] int sectorNumber) =>
        Respond(_controller.MoveToSector(enrollmentOfficerId, sectorNumber));
}
