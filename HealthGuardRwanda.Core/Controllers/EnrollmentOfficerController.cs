using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Services;

namespace HealthGuardRwanda.Core.Controllers;

public class EnrollmentOfficerController : ControllerBase
{
    private readonly EnrollmentOfficerService _service;

    public EnrollmentOfficerController(EnrollmentOfficerService service)
    {
        _service = service;
    }

    public ServiceResult Save(EnrollmentOfficer entity)
    {
        var message = _service.Save(entity);
        if (message == "That sector is taken by another enrollment officer")
            return Response(409, message);
        return Response(201, message);
    }

    public ServiceResult GetAll() => Ok(_service.GetAll());

    public ServiceResult GetById(Guid id)
    {
        var entity = _service.GetById(id);
        return entity is null
            ? Response(404, "The enrollment officer with that id does not exist")
            : Ok(entity);
    }

    public ServiceResult Update(Guid id, EnrollmentOfficer entity)
    {
        var message = _service.Update(id, entity);
        if (message == "The enrollment officer with that id does not exist")
            return Response(404, message);
        if (message == "That sector is taken by another enrollment officer")
            return Response(409, message);
        return Response(200, message);
    }

    public ServiceResult Delete(Guid id)
    {
        var message = _service.Delete(id);
        if (message == "The enrollment officer with that id does not exist")
            return Response(404, message);
        return Response(200, message);
    }

    public ServiceResult FindByEmail(string email)
    {
        var entity = _service.FindByEmail(email);
        return entity is null
            ? Response(404, "The enrollment officer with that email does not exist")
            : Ok(entity);
    }

    public ServiceResult ByHouseholdCategory(string name) => Ok(_service.FindByHouseholdCategoryName(name));

    public ServiceResult AddHouseholdCategory(Guid officerId, Guid categoryId)
    {
        var message = _service.AddHouseholdCategory(officerId, categoryId);
        if (message == "The enrollment officer with that id does not exist")
            return Response(404, message);
        if (message == "That household category does not exist")
            return Response(404, message);
        if (message == "The enrollment officer already has that household category")
            return Response(409, message);
        return Response(200, message);
    }

    public ServiceResult MoveToSector(Guid officerId, int sectorNumber)
    {
        var message = _service.MoveToSector(officerId, sectorNumber);
        if (message == "The enrollment officer with that id does not exist")
            return Response(404, message);
        if (message == "The sector with that sectorNumber does not exist")
            return Response(404, message);
        if (message == "That sector is taken by another enrollment officer")
            return Response(409, message);
        return Response(200, message);
    }
}
