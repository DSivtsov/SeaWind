using Api.Identity;
using Application.Models;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Identity;

namespace Api.UnitTests.Identity;

public class EmailFormatValidatorTests
{
    private readonly IFixture _fixture;
    private readonly EmailFormatValidator _emailFormatValidator;
    private readonly UserManager<AppUser> _manager;

    public EmailFormatValidatorTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _emailFormatValidator = _fixture.Build<EmailFormatValidator>().OmitAutoProperties().Create();
        _manager = _fixture.Build<UserManager<AppUser>>().OmitAutoProperties().Create();
    }

    [Theory]
    [InlineData("john.doe@example.com")]
    [InlineData("johndoe@example.com")]
    [InlineData("johndoe@ex.co")]
    [InlineData("юзер@пример.рф")]
    public async Task EmailFormatValidator_LocalPartIsValid_ReturnsTrue(string local)
    {
        // Arrange
        AppUser user = _fixture.Build<AppUser>()
            .With(u => u.Email, local)
            .Create();

        // Act
        var result = await _emailFormatValidator.ValidateAsync(_manager, user);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task EmailFormatValidator_QuotedLocalPartIsValid_ReturnsTrue()
    {
        // Arrange
        AppUser user = _fixture.Build<AppUser>()
            .With(u => u.Email, "\"john.doe\"@example.com")
            .Create();

        // Act
        var result = await _emailFormatValidator.ValidateAsync(_manager, user);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData("")]
    [InlineData("tyutu@gjhgj")]
    [InlineData("@example.com")]
    [InlineData("example@example.c")]
    [InlineData("user@-example.com")]
    public async Task EmailFormatValidator_LocalPartIsValid_ReturnsFalse(string? local)
    {
        // Arrange
        AppUser user = _fixture.Build<AppUser>()
            .With(u => u.Email, local)
            .Create();

        // Act
        var result = await _emailFormatValidator.ValidateAsync(_manager, user);

        // Assert
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task EmailFormatValidator_LocalPartTooLongIsValid_ReturnsFalse()
    {
        // Arrange
        AppUser user = _fixture.Build<AppUser>()
            .With(u => u.Email, new string('a', 65) + "@example.com")
            .Create();

        // Act
        var result = await _emailFormatValidator.ValidateAsync(_manager, user);

        // Assert
        Assert.False(result.Succeeded);
    }
}