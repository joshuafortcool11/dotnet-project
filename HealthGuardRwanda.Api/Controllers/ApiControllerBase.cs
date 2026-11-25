using Microsoft.AspNetCore.Mvc;
using ServiceResult = HealthGuardRwanda.Core.Controllers.ServiceResult;

namespace HealthGuardRwanda.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult Respond(ServiceResult result) =>
        StatusCode(result.Status, result.Data ?? result.Message);
}
