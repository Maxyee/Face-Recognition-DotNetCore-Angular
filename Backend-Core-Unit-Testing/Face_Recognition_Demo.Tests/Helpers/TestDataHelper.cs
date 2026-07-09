using System.Security.Claims;
using Face_Recognition_Demo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Face_Recognition_Demo.Tests.Helpers
{
    public static class TestDataHelper
    {
        public static User GetTestUser()
        {
            return new User
            {
                Id = "test-user-id-123",
                UserName = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
        }

        public static List<string> GetTestRoles()
        {
            return new List<string> { "User", "Admin" };
        }

        public static ClaimsPrincipal GetTestClaimsPrincipal()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id-123"),
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, "Test");
            return new ClaimsPrincipal(identity);
        }

        public static IConfiguration GetTestConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Secret", "YourSuperSecretKeyThatMustBeAtLeast32CharactersLong!"},
                {"Jwt:Issuer", "http://localhost:5000"},
                {"Jwt:Audience", "http://localhost:5000"},
                {"Jwt:ExpirationInMinutes", "60"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }
    }
}