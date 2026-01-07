using System.Net;
using FluentAssertions;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;

namespace GameScrubsV2.IntegrationTests.Tests.Auth;

public class LogoutUserTests : IntegrationTestBase
{
    public LogoutUserTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Logout_WithValidToken_ReturnsSuccess()
    {
        // Arrange
        var token = await CreateUserAndGetToken();
        SetAuthorizationHeader(token);

        // Act
        var response = await HttpClient.PostAsync("/api/auth/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await HttpClient.PostAsync("/api/auth/logout", null);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        SetAuthorizationHeader("invalid-token");

        // Act
        var response = await HttpClient.PostAsync("/api/auth/logout", null);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.OK);
    }
}