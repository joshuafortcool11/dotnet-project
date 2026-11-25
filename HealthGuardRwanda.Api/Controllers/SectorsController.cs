using HealthGuardRwanda.Core.Models;
using Microsoft.AspNetCore.Mvc;
using CoreControllers = HealthGuardRwanda.Core.Controllers;

namespace HealthGuardRwanda.Api.Controllers;

[ApiController]
[Route("api/sectors")]
public sealed class SectorsController : ApiControllerBase
{
    private readonly CoreControllers.SectorController _controller;

    public SectorsController(CoreControllers.SectorController controller)
    {
        _controller = controller;
    }

    [HttpPost("save")]
    public IActionResult Save([FromBody] Sector entity) => Respond(_controller.Save(entity));

    [HttpGet("all")]
    public IActionResult All() => Respond(_controller.GetAll());

    [HttpGet("free")]
    public IActionResult Free() => Respond(_controller.Free());

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id) => Respond(_controller.GetById(id));

    [HttpPut("update/{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] Sector entity) => Respond(_controller.Update(id, entity));

    [HttpDelete("delete/{id:guid}")]
    public IActionResult Delete(Guid id) => Respond(_controller.Delete(id));
}
