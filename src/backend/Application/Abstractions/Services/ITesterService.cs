using Application.DtoTester;

namespace Application.Abstractions.Services;

public interface ITesterService
{
    IEnumerable<TesterDto> GetAll(int? minAge);
    TesterDto? Get(Guid id);
    (bool IsBadReq, bool IsConflict, string? Error, TesterDto? Value) Create(CreateTesterRequest req);
    bool Update(Guid id, UpdateTesterRequest req);
    bool Delete(Guid id);
}
