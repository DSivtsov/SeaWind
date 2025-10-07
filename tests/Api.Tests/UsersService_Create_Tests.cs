using Application.Abstractions.Repositories;
using Application.DtoTester;
using Application.Models;
using Application.UseCasesTester;

namespace Api.Tests.Users
{
    // Simple in-memory fake repository for tests (no concurrency needed here)
    internal sealed class FakeUsersRepository : ITesterRepository
    {
        private readonly Dictionary<Guid, Tester> _db = new();

        public IEnumerable<Tester> GetAll() => _db.Values;

        public Tester? Find(Guid id) => _db.TryGetValue(id, out var u) ? u : default(Tester?);

        public void Save(Tester user) => _db[user.Id] = user;

        public bool Update(Tester user)
        {
            if (!_db.ContainsKey(user.Id)) return false;
            _db[user.Id] = user;
            return true;
        }

        public bool Delete(Guid id) => _db.Remove(id);
    }

    public sealed class UsersService_Create_Tests
    {
        private static TesterService CreateServiceWith(params Tester[] seed)
        {
            var repo = new FakeUsersRepository();
            foreach (var u in seed) repo.Save(u);
            return new TesterService(repo);
        }

        [Fact]
        public void Create_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var svc = CreateServiceWith();
            var req = new CreateTesterRequest("", -1);

            // Act
            var (bad, conflict, err, dto) = svc.Create(req);

            // Assert
            Assert.True(bad);
            Assert.False(conflict);
            Assert.NotNull(err);
            Assert.Null(dto);
        }

        [Fact]
        public void Create_DuplicateName_ReturnsConflict()
        {
            // Arrange
            var existing = new Tester(Guid.NewGuid(), "Alice", 30);
            var svc = CreateServiceWith(existing);
            var req = new CreateTesterRequest("Alice", 22);

            // Act
            var (bad, conflict, err, dto) = svc.Create(req);

            // Assert
            Assert.False(bad);
            Assert.True(conflict);
            Assert.NotNull(err);
            Assert.Null(dto);
        }

        [Fact]
        public void Create_Valid_ReturnsDtoAndPersists()
        {
            // Arrange
            var svc = CreateServiceWith();
            var req = new CreateTesterRequest("Bob", 25);

            // Act
            var (bad, conflict, err, dto) = svc.Create(req);

            // Assert
            Assert.False(bad);
            Assert.False(conflict);
            Assert.Null(err);
            Assert.NotNull(dto);
            Assert.Equal("Bob", dto!.Name);
            Assert.Equal(25, dto.Age);
            Assert.NotEqual(Guid.Empty, dto.Id);
        }
    }
}
