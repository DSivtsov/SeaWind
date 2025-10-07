using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DtoTester;
using Application.Models;

namespace Application.UseCasesTester;

internal sealed class TesterService : ITesterService
{
    private readonly ITesterRepository _repo;

    public TesterService(ITesterRepository repo) => _repo = repo;

    public IEnumerable<TesterDto> GetAll(int? minAge)
    {
        var users = _repo.GetAll();
        if (minAge is { } m) users = users.Where(u => u.Age >= m);
        return users.Select(u => new TesterDto(u.Id, u.Name, u.Age));
    }

    public TesterDto? Get(Guid id)
        => _repo.Find(id) is { } u ? new TesterDto(u.Id, u.Name, u.Age) : null;

    public (bool IsBadReq, bool IsConflict, string? Error, TesterDto? Value) Create(CreateTesterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || req.Age < 0)
            return (true, false, "Invalid user data", null);

        // Проверка уникальности теперь тут
        if (_repo.GetAll().Any(u => u.Name.Equals(req.Name, StringComparison.OrdinalIgnoreCase)))
            return (false, true, "User with this name already exists", null);

        var user = new Tester(Guid.NewGuid(), req.Name.Trim(), req.Age);
        _repo.Save(user);
        
        return (false, false, null, new TesterDto(user.Id, user.Name, user.Age));
    }

    public bool Update(Guid id, UpdateTesterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || req.Age < 0) return false;

        if (_repo.Find(id) is not Tester current) return false;

        // create updated snapshot (immutability-friendly)
        var updated = current with { Name = req.Name.Trim(), Age = req.Age };
        return _repo.Update(updated);
    }

    public bool Delete(Guid id) => _repo.Delete(id);
}
