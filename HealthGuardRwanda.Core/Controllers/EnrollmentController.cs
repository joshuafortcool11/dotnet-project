using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Services;

namespace HealthGuardRwanda.Core.Controllers;

public class EnrollmentController : ControllerBase
{
    private readonly EnrollmentService _service;

    public EnrollmentController(EnrollmentService service)
    {
        _service = service;
    }

    public ServiceResult Save(Enrollment entity)
    {
        var message = _service.Save(entity);
        if (message == "Enrollment date cannot be in the past")
            return Response(400, message);
        if (message == "Enrollment officer already processes an enrollment on that date")
            return Response(409, message);
        return Response(201, message);
    }

    public ServiceResult GetAll() => Ok(_service.GetAll());

    public ServiceResult GetById(Guid id)
    {
        var entity = _service.GetById(id);
        return entity is null
            ? Response(404, "The enrollment with that id does not exist")
            : Ok(entity);
    }

    public ServiceResult Update(Guid id, Enrollment entity)
    {
        var message = _service.Update(id, entity);
        if (message == "The enrollment with that id does not exist")
            return Response(404, message);
        return Response(200, message);
    }

    public ServiceResult Delete(Guid id)
    {
        var message = _service.Delete(id);
        if (message == "The enrollment with that id does not exist")
            return Response(404, message);
        if (message == "That enrollment has coverage verifications, delete them first")
            return Response(409, message);
        return Response(200, message);
    }

    public ServiceResult UpdateStatus(Guid id, EnrollmentStatus status)
    {
        var message = _service.UpdateStatus(id, status);
        if (message == "The enrollment with that id does not exist")
            return Response(404, message);
        if (message.StartsWith("Cannot change status"))
            return Response(400, message);
        return Response(200, message);
    }

    public ServiceResult ByStatus(EnrollmentStatus status) => Ok(_service.FindByStatusOrderByEnrollmentDateAsc(status));

    public ServiceResult Between(string start, string end) =>
        Ok(_service.FindBetweenEnrollmentDate(DateOnly.Parse(start), DateOnly.Parse(end)));

    public ServiceResult StatsByStatus() => Ok(_service.FindStatsByStatus());

    public ServiceResult CancelDay(Guid enrollmentOfficerId, string date) =>
        Response(200, _service.CancelDay(enrollmentOfficerId, DateOnly.Parse(date)));

    public ServiceResult Page(int page, int size, string sort)
    {
        var parts = (sort ?? "enrollmentDate,desc").Split(',');
        var sortProperty = parts[0] switch
        {
            "status" => "Status",
            "coverageYear" => "CoverageYear",
            "premium" => "Premium",
            _ => "EnrollmentDate"
        };
        var descending = parts.Length < 2 || !parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase);

        var request = new PageRequest
        {
            Page = page,
            Size = size,
            SortProperty = sortProperty,
            Descending = descending
        };
        return Ok(_service.FindPage(request));
    }
}
