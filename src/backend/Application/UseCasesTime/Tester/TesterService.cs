using Application.AbstractionsTime.Repositories;
using Application.AbstractionsTime.Services;
using Application.DtoTime.Tester;
using Application.ModelsTime;

namespace Application.UseCasesTime;

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

        var entity = _repo.Find(id);
        if (entity is null) return false;

        entity.Name = req.Name.Trim();
        entity.Age = req.Age;

        return _repo.Update(entity);
    }

    public bool Delete(Guid id) => _repo.Delete(id);
}
