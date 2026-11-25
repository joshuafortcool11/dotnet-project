using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Services;

namespace HealthGuardRwanda.Core.Controllers;

public class SectorController : ControllerBase
{
    private readonly SectorService _service;

    public SectorController(SectorService service)
    {
        _service = service;
    }

    public ServiceResult Save(Sector entity) => Response(201, _service.Save(entity));

    public ServiceResult GetAll() => Ok(_service.GetAll());

    public ServiceResult GetById(Guid id)
    {
        var entity = _service.GetById(id);
        return entity is null
            ? Response(404, "The sector with that id does not exist")
            : Ok(entity);
    }

    public ServiceResult Update(Guid id, Sector entity)
    {
        var message = _service.Update(id, entity);
        if (message == "The sector with that id does not exist")
            return Response(404, message);
        return Response(200, message);
    }

    public ServiceResult Delete(Guid id)
    {
        var message = _service.Delete(id);
        if (message == "The sector with that id does not exist")
            return Response(404, message);
        return Response(200, message);
    }

    public ServiceResult ByNumber(int sectorNumber)
    {
        var entity = _service.FindBySectorNumber(sectorNumber);
        return entity is null
            ? Response(404, "The sector with that sectorNumber does not exist")
            : Ok(entity);
    }

    public ServiceResult Free() => Ok(_service.FindFreeSectors());
}
