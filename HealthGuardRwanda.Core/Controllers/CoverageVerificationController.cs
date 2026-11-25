using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Services;

namespace HealthGuardRwanda.Core.Controllers;

public class CoverageVerificationController : ControllerBase
{
    private readonly CoverageVerificationService _service;

    public CoverageVerificationController(CoverageVerificationService service)
    {
        _service = service;
    }

    public ServiceResult Save(CoverageVerification entity)
    {
        var message = _service.Save(entity);
        if (message == "The enrollment with that id does not exist")
            return Response(404, message);
        if (message == "Coverage verifications are only for COVERED enrollments")
            return Response(400, message);
        return Response(201, message);
    }

    public ServiceResult GetAll() => Ok(_service.GetAll());

    public ServiceResult GetById(Guid id)
    {
        var entity = _service.GetById(id);
        return entity is null
            ? Response(404, "The coverage verification with that id does not exist")
            : Ok(entity);
    }

    public ServiceResult Update(Guid id, CoverageVerification entity)
    {
        var message = _service.Update(id, entity);
        if (message == "The coverage verification with that id does not exist")
            return Response(404, message);
        return Response(200, message);
    }

    public ServiceResult Delete(Guid id)
    {
        var message = _service.Delete(id);
        if (message == "The coverage verification with that id does not exist")
            return Response(404, message);
        return Response(200, message);
    }

    public ServiceResult ByCitizen(string nationalId) =>
        Ok(_service.FindByCitizenNationalIdOrderByVerifiedDateDesc(nationalId));
}
