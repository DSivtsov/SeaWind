using Application.Abstractions.Repositories;
using Application.Models;
using System.Collections.Concurrent;

namespace Infrastructure.InMemory.Repositories;

internal sealed class TesterRepository : ITesterRepository
{
    private readonly ConcurrentDictionary<Guid, Tester> _db = new();

    public IEnumerable<Tester> GetAll() => _db.Values;

    public Tester? Find(Guid id) => _db.TryGetValue(id, out var u) ? u : null;

    public void Save(Tester user) => _db[user.Id] = user;

    public bool Update(Tester user)
    {
        if (!_db.ContainsKey(user.Id)) return false;
        _db[user.Id] = user;
        return true;
    }

    public bool Delete(Guid id) => _db.TryRemove(id, out _);
}
