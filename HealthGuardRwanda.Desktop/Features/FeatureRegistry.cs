using HealthGuardRwanda.Core.Controllers;
using HealthGuardRwanda.Core.Models;

namespace HealthGuardRwanda.Desktop.Features;

public sealed class FeatureRegistry
{
    private readonly HouseholdCategoryController _categories;
    private readonly SectorController _sectors;
    private readonly EnrollmentOfficerController _officers;
    private readonly CitizenController _citizens;
    private readonly EnrollmentController _enrollments;
    private readonly CoverageVerificationController _verifications;
    private readonly DashboardController _dashboard;
    private readonly SeedController _seed;

    public FeatureRegistry(
        HouseholdCategoryController categories,
        SectorController sectors,
        EnrollmentOfficerController officers,
        CitizenController citizens,
        EnrollmentController enrollments,
        CoverageVerificationController verifications,
        DashboardController dashboard,
        SeedController seed)
    {
        _categories = categories;
        _sectors = sectors;
        _officers = officers;
        _citizens = citizens;
        _enrollments = enrollments;
        _verifications = verifications;
        _dashboard = dashboard;
        _seed = seed;
    }

    public List<Feature> Build()
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.Now).AddDays(1).ToString("yyyy-MM-dd");
        var features = new List<Feature>();

        features.Add(new Feature(
            "SEED", "Load seed data", "Setup",
            "Inserts categories, sectors, officers, citizens, enrollments and verifications once.",
            Array.Empty<FeatureParameter>(), _ => _seed.Load()));

        var categoryParams = new[] { new FeatureParameter("name", defaultValue: "Ubudehe 1") };
        features.AddRange(CrudFeatures<HouseholdCategory>(
            "A2-HC", "HouseholdCategory", "A - Domain & CRUD", categoryParams, "householdCategory",
            (p, e) => BuildCategory(p, (HouseholdCategory)e),
            p => _categories.GetById(Guid.Parse(p["id"])),
            () => _categories.GetAll(),
            e => _categories.Save((HouseholdCategory)e),
            (id, e) => _categories.Update(id, (HouseholdCategory)e),
            id => _categories.Delete(id)));

        var sectorParams = new[]
        {
            new FeatureParameter("sectorNumber", defaultValue: "4"),
            new FeatureParameter("name", defaultValue: "Kigali Sector 4")
        };
        features.AddRange(CrudFeatures<Sector>(
            "A2-SEC", "Sector", "A - Domain & CRUD", sectorParams, "sector",
            (p, e) => BuildSector(p, (Sector)e),
            p => _sectors.GetById(Guid.Parse(p["id"])),
            () => _sectors.GetAll(),
            e => _sectors.Save((Sector)e),
            (id, e) => _sectors.Update(id, (Sector)e),
            id => _sectors.Delete(id)));

        var officerParams = new[]
        {
            new FeatureParameter("firstName", defaultValue: "Jean"),
            new FeatureParameter("lastName", defaultValue: "Mugabo"),
            new FeatureParameter("email", defaultValue: "jean.mugabo@healthguard.rw"),
            new FeatureParameter("sectorNumber", defaultValue: "1"),
            new FeatureParameter("householdCategoryNames", defaultValue: "Ubudehe 1, Ubudehe 2")
        };
        features.AddRange(CrudFeatures<EnrollmentOfficer>(
            "A2-OFF", "EnrollmentOfficer", "A - Domain & CRUD", officerParams, "enrollmentOfficer",
            (p, e) => BuildOfficer(p, (EnrollmentOfficer)e),
            p => _officers.GetById(Guid.Parse(p["id"])),
            () => _officers.GetAll(),
            e => _officers.Save((EnrollmentOfficer)e),
            (id, e) => _officers.Update(id, (EnrollmentOfficer)e),
            id => _officers.Delete(id)));

        var citizenParams = new[]
        {
            new FeatureParameter("firstName", defaultValue: "Joshua"),
            new FeatureParameter("lastName", defaultValue: "Iradukunda"),
            new FeatureParameter("nationalId", defaultValue: "1198880123456789"),
            new FeatureParameter("phone", defaultValue: "0780000001"),
            new FeatureParameter("village", defaultValue: "Nyagatovu")
        };
        features.AddRange(CrudFeatures<Citizen>(
            "A2-CIT", "Citizen", "A - Domain & CRUD", citizenParams, "citizen",
            (p, e) => BuildCitizen(p, (Citizen)e),
            p => _citizens.GetById(Guid.Parse(p["id"])),
            () => _citizens.GetAll(),
            e => _citizens.Save((Citizen)e),
            (id, e) => _citizens.Update(id, (Citizen)e),
            id => _citizens.Delete(id)));

        var enrollmentParams = new[]
        {
            new FeatureParameter("citizenNationalId", defaultValue: "1198880123456789"),
            new FeatureParameter("officerEmail", defaultValue: "jean.mugabo@healthguard.rw"),
            new FeatureParameter("enrollmentDate", defaultValue: tomorrow),
            new FeatureParameter("coverageYear", defaultValue: "2026"),
            new FeatureParameter("premium", defaultValue: "3000"),
            new FeatureParameter("status", defaultValue: "APPLIED")
        };
        features.AddRange(CrudFeatures<Enrollment>(
            "D1", "Enrollment", "D - Business rules", enrollmentParams, "enrollment",
            (p, e) => BuildEnrollment(p, (Enrollment)e),
            p => _enrollments.GetById(Guid.Parse(p["id"])),
            () => _enrollments.GetAll(),
            e => _enrollments.Save((Enrollment)e),
            (id, e) => _enrollments.Update(id, (Enrollment)e),
            id => _enrollments.Delete(id)));

        var verificationParams = new[]
        {
            new FeatureParameter("enrollmentId", "coveredEnrollment"),
            new FeatureParameter("healthCenter", defaultValue: "Kimironko Health Center"),
            new FeatureParameter("remarks", defaultValue: "Coverage valid, consultation approved"),
            new FeatureParameter("dependents", defaultValue: "3")
        };
        features.AddRange(CrudFeatures<CoverageVerification>(
            "E1", "CoverageVerification", "E - Coverage verification", verificationParams, "coverageVerification",
            (p, e) => BuildVerification(p, (CoverageVerification)e),
            p => _verifications.GetById(Guid.Parse(p["id"])),
            () => _verifications.GetAll(),
            e => _verifications.Save((CoverageVerification)e),
            (id, e) => _verifications.Update(id, (CoverageVerification)e),
            id => _verifications.Delete(id)));

        features.Add(new Feature("B1", "Citizens by village", "B - Derived queries",
            "IgnoreCase + OrderBy lastName ASC",
            new[] { new FeatureParameter("village", defaultValue: "nyagatovu") },
            p => _citizens.ByVillage(p["village"])));

        features.Add(new Feature("B2", "Enrollments by status", "B - Derived queries",
            "Enum parameter + OrderBy enrollmentDate ASC",
            new[] { new FeatureParameter("status", defaultValue: "APPLIED") },
            p => _enrollments.ByStatus(Enum.Parse<EnrollmentStatus>(p["status"], true))));

        features.Add(new Feature("B3", "Enrollments between dates", "B - Derived queries",
            "Both dates inclusive, ordered by enrollmentDate",
            new[]
            {
                new FeatureParameter("start", defaultValue: "2026-10-01"),
                new FeatureParameter("end", defaultValue: "2026-10-31")
            },
            p => _enrollments.Between(p["start"], p["end"])));

        features.Add(new Feature("C1", "Officers by household category", "C - LINQ queries with joins",
            "LINQ query on the categories collection + LOWER",
            new[] { new FeatureParameter("name", defaultValue: "ubudehe 1") },
            p => _officers.ByHouseholdCategory(p["name"])));

        features.Add(new Feature("C2", "Free sectors", "C - LINQ queries with joins",
            "NOT EXISTS equivalent, ordered by sectorNumber",
            Array.Empty<FeatureParameter>(), _ => _sectors.Free()));

        features.Add(new Feature("C3", "Unused household categories", "C - LINQ queries with joins",
            "Empty officers collection (IS EMPTY equivalent)",
            Array.Empty<FeatureParameter>(), _ => _categories.Unused()));

        features.Add(new Feature("C4", "Citizens of an enrollment officer", "C - LINQ queries with joins",
            "DISTINCT + navigation through EnrollmentOfficer",
            new[] { new FeatureParameter("enrollmentOfficerId", "enrollmentOfficer") },
            p => _citizens.OfEnrollmentOfficer(Guid.Parse(p["enrollmentOfficerId"]))));

        features.Add(new Feature("D2", "Change enrollment status", "D - Business rules",
            "APPLIED -> PAYMENT_PENDING/REJECTED, PAYMENT_PENDING -> COVERED/REJECTED",
            new[]
            {
                new FeatureParameter("id", "enrollment"),
                new FeatureParameter("status", defaultValue: "PAYMENT_PENDING")
            },
            p => _enrollments.UpdateStatus(Guid.Parse(p["id"]), Enum.Parse<EnrollmentStatus>(p["status"], true))));

        features.Add(new Feature("D3", "Add household category to officer", "D - Business rules",
            "Many-to-many list add, keeps existing categories",
            new[]
            {
                new FeatureParameter("enrollmentOfficerId", "enrollmentOfficer"),
                new FeatureParameter("householdCategoryId", "householdCategory")
            },
            p => _officers.AddHouseholdCategory(
                Guid.Parse(p["enrollmentOfficerId"]), Guid.Parse(p["householdCategoryId"]))));

        features.Add(new Feature("D4", "Move officer to another sector", "D - Business rules",
            "Free sector becomes occupied; old sector becomes free",
            new[]
            {
                new FeatureParameter("enrollmentOfficerId", "enrollmentOfficer"),
                new FeatureParameter("sectorNumber", defaultValue: "4")
            },
            p => _officers.MoveToSector(
                Guid.Parse(p["enrollmentOfficerId"]), int.Parse(p["sectorNumber"]))));

        features.Add(new Feature("E2", "Coverage history of a citizen", "E - Coverage verification",
            "Navigation via enrollment.citizen, newest verifiedDate first",
            new[] { new FeatureParameter("nationalId", defaultValue: "1198880123456789") },
            p => _verifications.ByCitizen(p["nationalId"])));

        features.Add(new Feature("E3", "Delete coverage verification", "E - Coverage verification",
            "Also protects an enrollment that still has verifications",
            new[] { new FeatureParameter("id", "coverageVerification") },
            p => _verifications.Delete(Guid.Parse(p["id"]))));

        features.Add(new Feature("E3-ENC", "Try deleting enrollment with verifications", "E - Coverage verification",
            "Expect 409 when verifications still exist",
            new[] { new FeatureParameter("id", "enrollment") },
            p => _enrollments.Delete(Guid.Parse(p["id"]))));

        features.Add(new Feature("F1", "Enrollments per status", "F - Reports & bulk updates",
            "GROUP BY + COUNT as List<Object[]>",
            Array.Empty<FeatureParameter>(), _ => _enrollments.StatsByStatus()));

        features.Add(new Feature("F2", "Enrollment officer absent", "F - Reports & bulk updates",
            "One UPDATE query, never touches COVERED enrollments",
            new[]
            {
                new FeatureParameter("enrollmentOfficerId", "enrollmentOfficer"),
                new FeatureParameter("date", defaultValue: "2026-10-20")
            },
            p => _enrollments.CancelDay(Guid.Parse(p["enrollmentOfficerId"]), p["date"])));

        features.Add(new Feature("BONUS-1", "Dashboard summary", "Bonus",
            "Map<String, Long> with insertion order preserved",
            Array.Empty<FeatureParameter>(), _ => _dashboard.Summary()));

        features.Add(new Feature("BONUS-2", "Enrollments page by page", "Bonus",
            "Pageable built in the controller from page, size and sort",
            new[]
            {
                new FeatureParameter("page", defaultValue: "0"),
                new FeatureParameter("size", defaultValue: "5"),
                new FeatureParameter("sort", defaultValue: "enrollmentDate,desc")
            },
            p => _enrollments.Page(int.Parse(p["page"]), int.Parse(p["size"]), p["sort"])));

        return features;
    }

    private static List<Feature> CrudFeatures<T>(
        string id,
        string title,
        string group,
        IReadOnlyList<FeatureParameter> writeParams,
        string picker,
        Func<IReadOnlyDictionary<string, string>, object, (ServiceResult? Error, object Entity)> build,
        Func<IReadOnlyDictionary<string, string>, ServiceResult> getById,
        Func<ServiceResult> getAll,
        Func<object, ServiceResult> save,
        Func<Guid, object, ServiceResult> update,
        Func<Guid, ServiceResult> delete) where T : new()
    {
        var withId = writeParams.Append(new FeatureParameter("id", picker)).ToArray();
        var idOnly = new[] { new FeatureParameter("id", picker) };

        ServiceResult Save(IReadOnlyDictionary<string, string> p)
        {
            var (error, entity) = build(p, new T());
            return error ?? save(entity);
        }

        ServiceResult Update(IReadOnlyDictionary<string, string> p)
        {
            var (error, entity) = build(p, new T());
            return error ?? update(Guid.Parse(p["id"]), entity);
        }

        return new List<Feature>
        {
            new($"{id}-SAVE", $"{title} - Save", group, "POST /save equivalent", writeParams, Save),
            new($"{id}-ALL", $"{title} - All", group, "GET /all equivalent", Array.Empty<FeatureParameter>(), _ => getAll()),
            new($"{id}-GET", $"{title} - Get by id", group, "GET /{id} equivalent", idOnly, p => getById(p)),
            new($"{id}-UPDATE", $"{title} - Update", group, "PUT /update/{id} equivalent", withId, Update),
            new($"{id}-DELETE", $"{title} - Delete", group, "DELETE /delete/{id} equivalent", idOnly, p => delete(Guid.Parse(p["id"])))
        };
    }

    private static (ServiceResult? Error, object Entity) BuildCategory(
        IReadOnlyDictionary<string, string> p, HouseholdCategory entity)
    {
        entity.Name = p["name"];
        return (null, entity);
    }

    private static (ServiceResult? Error, object Entity) BuildSector(
        IReadOnlyDictionary<string, string> p, Sector entity)
    {
        entity.SectorNumber = int.Parse(p["sectorNumber"]);
        entity.Name = p["name"];
        return (null, entity);
    }

    private (ServiceResult? Error, object Entity) BuildOfficer(
        IReadOnlyDictionary<string, string> p, EnrollmentOfficer entity)
    {
        entity.FirstName = p["firstName"];
        entity.LastName = p["lastName"];
        entity.Email = p["email"];

        if (int.TryParse(p.GetValueOrDefault("sectorNumber"), out var sectorNumber))
        {
            var sectorResult = _sectors.ByNumber(sectorNumber);
            if (!sectorResult.Success)
                return (sectorResult, entity);
            entity.Sector = (Sector)sectorResult.Data!;
            entity.SectorId = entity.Sector.Id;
        }

        entity.HouseholdCategories = new List<HouseholdCategory>();
        var names = p.GetValueOrDefault("householdCategoryNames", string.Empty);
        foreach (var raw in names.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var categoryResult = _categories.FindByName(raw);
            if (!categoryResult.Success)
                return (categoryResult, entity);
            entity.HouseholdCategories.Add((HouseholdCategory)categoryResult.Data!);
        }

        return (null, entity);
    }

    private static (ServiceResult? Error, object Entity) BuildCitizen(
        IReadOnlyDictionary<string, string> p, Citizen entity)
    {
        entity.FirstName = p["firstName"];
        entity.LastName = p["lastName"];
        entity.NationalId = p["nationalId"];
        entity.Phone = p["phone"];
        entity.Village = p["village"];
        return (null, entity);
    }

    private (ServiceResult? Error, object Entity) BuildEnrollment(
        IReadOnlyDictionary<string, string> p, Enrollment entity)
    {
        var citizenResult = _citizens.FindByNationalId(p["citizenNationalId"]);
        if (!citizenResult.Success)
            return (citizenResult, entity);
        var citizen = (Citizen)citizenResult.Data!;

        var officerResult = _officers.FindByEmail(p["officerEmail"]);
        if (!officerResult.Success)
            return (officerResult, entity);
        var officer = (EnrollmentOfficer)officerResult.Data!;

        entity.Citizen = citizen;
        entity.CitizenId = citizen.Id;
        entity.EnrollmentOfficer = officer;
        entity.EnrollmentOfficerId = officer.Id;
        entity.EnrollmentDate = DateOnly.Parse(p["enrollmentDate"]);
        entity.CoverageYear = int.Parse(p["coverageYear"]);
        entity.Premium = int.Parse(p["premium"]);
        entity.Status = Enum.Parse<EnrollmentStatus>(p["status"], true);
        return (null, entity);
    }

    private (ServiceResult? Error, object Entity) BuildVerification(
        IReadOnlyDictionary<string, string> p, CoverageVerification entity)
    {
        if (!Guid.TryParse(p.GetValueOrDefault("enrollmentId"), out var enrollmentId))
            return (new ServiceResult { Status = 400, Message = "Choose a COVERED enrollment" }, entity);

        var enrollmentResult = _enrollments.GetById(enrollmentId);
        if (!enrollmentResult.Success)
            return (enrollmentResult, entity);
        var enrollment = (Enrollment)enrollmentResult.Data!;

        entity.Enrollment = enrollment;
        entity.EnrollmentId = enrollment.Id;
        entity.HealthCenter = p["healthCenter"];
        entity.Remarks = p["remarks"];
        entity.Dependents = int.Parse(p["dependents"]);
        return (null, entity);
    }
}
