using HealthGuardRwanda.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthGuardRwanda.Core.Repositories;

public interface IRepository<T> where T : class
{
    T? GetById(Guid id);
    List<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    void Detach(T entity);
    void SaveChanges();
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Db;

    public Repository(AppDbContext db)
    {
        Db = db;
    }

    public virtual T? GetById(Guid id) => Db.Set<T>().Find(id);

    public virtual List<T> GetAll() => Db.Set<T>().ToList();

    public void Add(T entity) => Db.Set<T>().Add(entity);

    public void Update(T entity) => Db.Set<T>().Update(entity);

    public void Remove(T entity) => Db.Set<T>().Remove(entity);

    public void Detach(T entity) => Db.Entry(entity).State = EntityState.Detached;

    public void SaveChanges() => Db.SaveChanges();
}
