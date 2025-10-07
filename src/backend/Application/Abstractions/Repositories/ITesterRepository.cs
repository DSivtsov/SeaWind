using Application.Models;

namespace Application.Abstractions.Repositories;
public interface ITesterRepository
{
    IEnumerable<Tester> GetAll();
    Tester? Find(Guid id);
    void Save(Tester user);       // store ready object
    bool Update(Tester user);     // update by whole object
    bool Delete(Guid id);       // delete by id
}
