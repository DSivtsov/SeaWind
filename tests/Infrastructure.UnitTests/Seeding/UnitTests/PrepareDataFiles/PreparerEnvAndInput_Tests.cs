using FluentAssertions;
using Infrastructure.Postgres.Seeding.SeedDataFiles;

namespace Infrastructure.UnitTests.Seeding.UnitTests.PrepareDataFiles;

/// <summary>
/// Набор юнит-тестов для <see cref="SeedEnvironmentPreparer"/>,
/// проверяющий корректность подготовки окружения для сидирования:
/// валидацию пути к каталогу и поиск входных seed-файлов.
/// </summary>
public class PreparerEnvAndInput_Tests
{
    [Fact]
    public void PreparerEnvAndInput_Absent_Directory_SeedFiles_ThrowException()
    {
        //Arrange
        var pathNonExistingDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        var sut = new SeedEnvironmentPreparer(pathNonExistingDir);

        // Act
        Action act = () => sut.PrepareEnvironmentAndLocateFiles();

        // Assert
        act.Should()
           .Throw<InvalidDataException>()
           .WithMessage("Directory for seed files not found*");
    }

    [Fact]
    public void PreparerEnvAndInput_Wrong_Directory_ThrowException()
    {
        //Arrange
        var pathWrong = " ";

        var sut = new SeedEnvironmentPreparer(pathWrong);

        // Act
        Action act = () => sut.PrepareEnvironmentAndLocateFiles();

        // Assert
        act.Should()
           .Throw<InvalidDataException>()
           .WithMessage("Wrong directory*");
        // * в конце, т.к. в сообщении ещё будет [...pathBase]
    }

    [Fact]
    public void PreparerEnvAndInput_Absent_SeedFiles_ThrowException()
    {
        //Arrange
        var pathNoSeedFile = Directory.CreateTempSubdirectory().FullName;

        var sut = new SeedEnvironmentPreparer(pathNoSeedFile);

        // Act
        Action act = () => sut.PrepareEnvironmentAndLocateFiles();

        // Assert
        act.Should()
           .Throw<InvalidDataException>()
           .WithMessage("No seed files in directory*");
    }

    [Fact]
    public void Prepare_SeedFilesExist_ReturnsOrderedFilePaths()
    {
        var tempDir = Directory.CreateTempSubdirectory();

        var fileB = Path.Combine(tempDir.FullName, "b.seed.json");
        var fileA = Path.Combine(tempDir.FullName, "a.seed.json");

        File.WriteAllText(fileB, "[]");
        File.WriteAllText(fileA, "[]");

        var sut = new SeedEnvironmentPreparer(tempDir.FullName);

        string ROOT_FOLDER = "./";
        var expected = new[] {
                Path.Combine(ROOT_FOLDER, "b.seed.json"),
                Path.Combine(ROOT_FOLDER, "a.seed.json")}
            .OrderBy(fileName => fileName, StringComparer.Ordinal);

        // Act
        var result = sut.PrepareEnvironmentAndLocateFiles().ToArray();

        // Assert
        result.Should()
              .HaveCount(2)
              .And.ContainInOrder(expected);
    }
}
