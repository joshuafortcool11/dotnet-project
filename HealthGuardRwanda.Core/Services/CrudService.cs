using HealthGuardRwanda.Core.Repositories;

namespace HealthGuardRwanda.Core.Services;

public sealed class EntityMeta
{
    public EntityMeta(string displayName, string messageName)
    {
        DisplayName = displayName;
        MessageName = messageName;
    }

    public string DisplayName { get; }
    public string MessageName { get; }
}

public class CrudService<T> where T : class
{
    protected readonly IRepository<T> Repo;
    protected readonly EntityMeta Meta;

    public CrudService(IRepository<T> repo, EntityMeta meta)
    {
        Repo = repo;
        Meta = meta;
    }

    public virtual string Save(T entity)
    {
        Repo.Add(entity);
        Repo.SaveChanges();
        return $"{Meta.DisplayName} saved successfully";
    }

    public List<T> GetAll() => Repo.GetAll();

    public T? GetById(Guid id) => Repo.GetById(id);

    public virtual string Update(Guid id, T entity)
    {
        var existing = Repo.GetById(id);
        if (existing is null)
            return $"The {Meta.MessageName} with that id does not exist";

        Repo.Detach(existing);
        typeof(T).GetProperty("Id")!.SetValue(entity, id);
        Repo.Update(entity);
        Repo.SaveChanges();
        return $"{Meta.DisplayName} updated successfully";
    }

    public virtual string Delete(Guid id)
    {
        var existing = Repo.GetById(id);
        if (existing is null)
            return $"The {Meta.MessageName} with that id does not exist";

        Repo.Remove(existing);
        Repo.SaveChanges();
        return $"{Meta.DisplayName} deleted successfully";
    }
}
