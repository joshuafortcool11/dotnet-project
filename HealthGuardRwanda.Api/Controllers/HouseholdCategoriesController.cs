using HealthGuardRwanda.Core.Models;
using Microsoft.AspNetCore.Mvc;
using CoreControllers = HealthGuardRwanda.Core.Controllers;

namespace HealthGuardRwanda.Api.Controllers;

[ApiController]
[Route("api/household-categories")]
public sealed class HouseholdCategoriesController : ApiControllerBase
{
    private readonly CoreControllers.HouseholdCategoryController _controller;

    public HouseholdCategoriesController(CoreControllers.HouseholdCategoryController controller)
    {
        _controller = controller;
    }

    [HttpPost("save")]
    public IActionResult Save([FromBody] HouseholdCategory entity) => Respond(_controller.Save(entity));

    [HttpGet("all")]
    public IActionResult All() => Respond(_controller.GetAll());

    [HttpGet("unused")]
    public IActionResult Unused() => Respond(_controller.Unused());

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id) => Respond(_controller.GetById(id));

    [HttpPut("update/{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] HouseholdCategory entity) => Respond(_controller.Update(id, entity));

    [HttpDelete("delete/{id:guid}")]
    public IActionResult Delete(Guid id) => Respond(_controller.Delete(id));
}
