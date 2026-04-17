using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Voltei.Api.Models;
using Voltei.Api.Services;

namespace Voltei.Api.UnitTests.Services;

public class TokenServiceTests
{
    private readonly TokenService _sut;
    private readonly User _user;
    private const string Secret = "TestSecretKeyThatIsAtLeast32BytesLong!!";
    private const string Issuer = "TestIssuer";
    private const string Audience = "TestAudience";

    public TokenServiceTests()
    {
        var config = Substitute.For<IConfiguration>();
        config["Jwt:Secret"].Returns(Secret);
        config["Jwt:Issuer"].Returns(Issuer);
        config["Jwt:Audience"].Returns(Audience);

        _sut = new TokenService(config);

        _user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Nome = "Test User",
            NegocioId = Guid.NewGuid(),
            SenhaHash = "hash",
        };
    }

    private JwtSecurityToken DecodeToken(string token)
        => new JwtSecurityTokenHandler().ReadJwtToken(token);

    [Fact]
    public void GenerateToken_ReturnsNonEmptyString()
    {
        var token = _sut.GenerateToken(_user);
        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateToken_ContainsSubClaim_WithUserId()
    {
        var token = _sut.GenerateToken(_user);
        var jwt = DecodeToken(token);
        jwt.Subject.Should().Be(_user.Id.ToString());
    }

    [Fact]
    public void GenerateToken_ContainsNegocioIdClaim()
    {
        var token = _sut.GenerateToken(_user);
        var jwt = DecodeToken(token);
        jwt.Claims.Should().Contain(c => c.Type == "negocioId" && c.Value == _user.NegocioId.ToString());
    }

    [Fact]
    public void GenerateToken_HasCorrectIssuer()
    {
        var token = _sut.GenerateToken(_user);
        var jwt = DecodeToken(token);
        jwt.Issuer.Should().Be(Issuer);
    }

    [Fact]
    public void GenerateToken_HasCorrectAudience()
    {
        var token = _sut.GenerateToken(_user);
        var jwt = DecodeToken(token);
        jwt.Audiences.Should().Contain(Audience);
    }

    [Fact]
    public void GenerateToken_ExpiresInSevenDays()
    {
        var before = DateTime.UtcNow.AddDays(7);
        var token = _sut.GenerateToken(_user);
        var after = DateTime.UtcNow.AddDays(7);

        var jwt = DecodeToken(token);
        jwt.ValidTo.Should().BeOnOrAfter(before.AddSeconds(-5));
        jwt.ValidTo.Should().BeOnOrBefore(after.AddSeconds(5));
    }

    [Fact]
    public void GenerateToken_UsesHmacSha256()
    {
        var token = _sut.GenerateToken(_user);
        var jwt = DecodeToken(token);
        jwt.SignatureAlgorithm.Should().Be("HS256");
    }
}
