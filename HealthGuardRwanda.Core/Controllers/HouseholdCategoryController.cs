using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Services;

namespace HealthGuardRwanda.Core.Controllers;

public class HouseholdCategoryController : ControllerBase
{
    private readonly HouseholdCategoryService _service;

    public HouseholdCategoryController(HouseholdCategoryService service)
    {
        _service = service;
    }

    public ServiceResult Save(HouseholdCategory entity)
    {
        var message = _service.Save(entity);
        return Response(201, message);
    }

    public ServiceResult GetAll() => Ok(_service.GetAll());

    public ServiceResult GetById(Guid id)
    {
        var entity = _service.GetById(id);
        return entity is null
            ? Response(404, "The household category with that id does not exist")
            : Ok(entity);
    }

    public ServiceResult Update(Guid id, HouseholdCategory entity)
    {
        var message = _service.Update(id, entity);
        if (message == "The household category with that id does not exist")
            return Response(404, message);
        return Response(200, message);
    }

    public ServiceResult Delete(Guid id)
    {
        var message = _service.Delete(id);
        if (message == "The household category with that id does not exist")
            return Response(404, message);
        return Response(200, message);
    }

    public ServiceResult FindByName(string name)
    {
        var entity = _service.FindByName(name);
        return entity is null
            ? Response(404, "That household category does not exist")
            : Ok(entity);
    }

    public ServiceResult Unused() => Ok(_service.FindUnused());
}
