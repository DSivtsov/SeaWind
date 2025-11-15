using Application.AbstractionsTime.Repositories;
using Application.ModelsTime;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Time.Repositories;

public sealed class TesterRepositoryPostgres : ITesterRepository
{
    private readonly TimeDbContext _db;
    public TesterRepositoryPostgres(TimeDbContext db) => _db = db;

    public IEnumerable<Tester> GetAll() => _db.Testers.AsNoTracking().ToList();
    public Tester? Find(Guid id) => _db.Testers.AsNoTracking().FirstOrDefault(x => x.Id == id);
    public void Save(Tester user) { _db.Testers.Add(user); _db.SaveChanges(); }
    public bool Update(Tester user)
    {
        if (!_db.Testers.Any(x => x.Id == user.Id)) return false;
        _db.Testers.Update(user); _db.SaveChanges(); return true;
    }
    public bool Delete(Guid id)
    {
        var stub = new Tester(id, "", 0);
        _db.Entry(stub).State = EntityState.Deleted;
        return _db.SaveChanges() > 0;
    }
}
