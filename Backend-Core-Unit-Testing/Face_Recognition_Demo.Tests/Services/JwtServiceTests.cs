using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Face_Recognition_Demo.Models;
using Face_Recognition_Demo.Services;
using Face_Recognition_Demo.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Face_Recognition_Demo.Tests.Services
{
    public class JwtServiceTests
    {
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly User _testUser;
        private readonly List<string> _testRoles;

        public JwtServiceTests()
        {
            _configuration = TestDataHelper.GetTestConfiguration();
            _jwtService = new JwtService(_configuration);
            _testUser = TestDataHelper.GetTestUser();
            _testRoles = TestDataHelper.GetTestRoles();
        }

        [Fact]
        public void GenerateToken_WithValidUser_ShouldReturnValidToken()
        {
            // Arrange
            var user = _testUser;
            var roles = _testRoles;

            // Act
            var token = _jwtService.GenerateToken(user, roles);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            jwtToken.Should().NotBeNull();
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.UserName);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == user.Email);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }

        [Fact]
        public void GenerateToken_WithEmptyRoles_ShouldGenerateTokenWithoutRoleClaims()
        {
            // Arrange
            var user = _testUser;
            var emptyRoles = new List<string>();

            // Act
            var token = _jwtService.GenerateToken(user, emptyRoles);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            jwtToken.Claims.Should().NotContain(c => c.Type == ClaimTypes.Role);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnValidBase64String()
        {
            // Act
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Assert
            refreshToken.Should().NotBeNullOrEmpty();
            refreshToken.Length.Should().Be(44); // Base64 of 32 bytes
            
            // Verify it's valid base64
            var converted = Convert.FromBase64String(refreshToken);
            converted.Length.Should().Be(32);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnUniqueTokens()
        {
            // Act
            var token1 = _jwtService.GenerateRefreshToken();
            var token2 = _jwtService.GenerateRefreshToken();

            // Assert
            token1.Should().NotBe(token2);
        }

        [Fact]
        public void GetPrincipalFromExpiredToken_WithValidToken_ShouldReturnClaimsPrincipal()
        {
            // Arrange
            var token = _jwtService.GenerateToken(_testUser, _testRoles);
            
            // Act
            var principal = _jwtService.GetPrincipalFromExpiredToken(token);

            // Assert
            principal.Should().NotBeNull();
            principal.Identity.Should().NotBeNull();
            principal.Identity.Name.Should().Be(_testUser.UserName);
            
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            userIdClaim.Should().NotBeNull();
            userIdClaim.Value.Should().Be(_testUser.Id);
        }

        [Fact]
        public void GetPrincipalFromExpiredToken_WithInvalidToken_ShouldThrowSecurityTokenException()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act & Assert
            Assert.Throws<SecurityTokenException>(() => 
                _jwtService.GetPrincipalFromExpiredToken(invalidToken));
        }

        [Fact]
        public void ValidateToken_WithValidToken_ShouldReturnTrue()
        {
            // Arrange
            var token = _jwtService.GenerateToken(_testUser, _testRoles);

            // Act
            var isValid = _jwtService.ValidateToken(token);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidateToken_WithInvalidToken_ShouldReturnFalse()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var isValid = _jwtService.ValidateToken(invalidToken);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateToken_WithEmptyToken_ShouldReturnFalse()
        {
            // Act
            var isValid = _jwtService.ValidateToken(string.Empty);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void GetTokenExpiration_WithValidToken_ShouldReturnExpirationDate()
        {
            // Arrange
            var token = _jwtService.GenerateToken(_testUser, _testRoles);
            var expectedExpiry = DateTime.UtcNow.AddMinutes(60);

            // Act
            var expiration = _jwtService.GetTokenExpiration(token);

            // Assert
            expiration.Should().NotBeNull();
            expiration.Value.Should().BeCloseTo(expectedExpiry, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void GetTokenExpiration_WithInvalidToken_ShouldReturnNull()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var expiration = _jwtService.GetTokenExpiration(invalidToken);

            // Assert
            expiration.Should().BeNull();
        }

        [Fact]
        public void GetUserIdFromToken_WithValidToken_ShouldReturnUserId()
        {
            // Arrange
            var token = _jwtService.GenerateToken(_testUser, _testRoles);

            // Act
            var userId = _jwtService.GetUserIdFromToken(token);

            // Assert
            userId.Should().NotBeNullOrEmpty();
            userId.Should().Be(_testUser.Id);
        }

        [Fact]
        public void GetUserIdFromToken_WithInvalidToken_ShouldReturnNull()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var userId = _jwtService.GetUserIdFromToken(invalidToken);

            // Assert
            userId.Should().BeNull();
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Manager")]
        public void GenerateToken_WithDifferentRoles_ShouldIncludeCorrectRoles(string role)
        {
            // Arrange
            var user = _testUser;
            var roles = new List<string> { role };

            // Act
            var token = _jwtService.GenerateToken(user, roles);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == role);
        }
    }
}