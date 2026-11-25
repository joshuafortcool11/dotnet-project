using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Services;

namespace HealthGuardRwanda.Core.Controllers;

public class CitizenController : ControllerBase
{
    private readonly CitizenService _service;

    public CitizenController(CitizenService service)
    {
        _service = service;
    }

    public ServiceResult Save(Citizen entity) => Response(201, _service.Save(entity));

    public ServiceResult GetAll() => Ok(_service.GetAll());

    public ServiceResult GetById(Guid id)
    {
        var entity = _service.GetById(id);
        return entity is null
            ? Response(404, "The citizen with that id does not exist")
            : Ok(entity);
    }

    public ServiceResult Update(Guid id, Citizen entity)
    {
        var message = _service.Update(id, entity);
        if (message == "The citizen with that id does not exist")
            return Response(404, message);
        return Response(200, message);
    }

    public ServiceResult Delete(Guid id)
    {
        var message = _service.Delete(id);
        if (message == "The citizen with that id does not exist")
            return Response(404, message);
        return Response(200, message);
    }

    public ServiceResult FindByNationalId(string nationalId)
    {
        var entity = _service.FindByNationalId(nationalId);
        return entity is null
            ? Response(404, "The citizen with that nationalId does not exist")
            : Ok(entity);
    }

    public ServiceResult ByVillage(string village) => Ok(_service.FindByVillageIgnoreCaseOrderByLastName(village));

    public ServiceResult OfEnrollmentOfficer(Guid officerId)
    {
        if (!_service.OfficerExists(officerId))
            return Response(404, "The enrollment officer with that id does not exist");
        return Ok(_service.FindCitizensOfEnrollmentOfficer(officerId));
    }
}
