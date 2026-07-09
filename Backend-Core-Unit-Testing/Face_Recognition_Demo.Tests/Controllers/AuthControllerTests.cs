using Face_Recognition_Demo.Controllers;
using Face_Recognition_Demo.DTOs;
using Face_Recognition_Demo.Models;
using Face_Recognition_Demo.Services;
using Face_Recognition_Demo.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Face_Recognition_Demo.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _userManagerMock = GetUserManagerMock();
            _roleManagerMock = GetRoleManagerMock();
            _signInManagerMock = GetSignInManagerMock();
            _jwtServiceMock = new Mock<IJwtService>();
            _loggerMock = new Mock<ILogger<AuthController>>();

            _controller = new AuthController(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _signInManagerMock.Object,
                _jwtServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Register_WithValidData_ShouldReturnOkResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "Test@123",
                ConfirmPassword = "Test@123",
                FirstName = "Test",
                LastName = "User",
                Role = "User"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { "User" });
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>()))
                .Returns("test-token");
            _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("test-refresh-token");

            _roleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeOfType<TokenResponseDto>();
        }

        [Fact]
        public async Task Register_WithExistingUsername_ShouldReturnBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Username = "existinguser",
                Password = "Test@123",
                ConfirmPassword = "Test@123",
                FirstName = "Test",
                LastName = "User"
            };

            var existingUser = new User { UserName = "existinguser" };
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOkResult()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "Test@123"
            };

            var user = TestDataHelper.GetTestUser();
            
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { "User" });
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>()))
                .Returns("test-token");
            _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("test-refresh-token");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeOfType<TokenResponseDto>();
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "WrongPassword"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task RefreshToken_WithValidTokens_ShouldReturnNewTokens()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDto
            {
                Token = "valid-token",
                RefreshToken = "valid-refresh-token"
            };

            var user = TestDataHelper.GetTestUser();
            var claimsPrincipal = TestDataHelper.GetTestClaimsPrincipal();

            _jwtServiceMock.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>()))
                .Returns(claimsPrincipal);
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { "User" });
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>()))
                .Returns("new-test-token");
            _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("new-test-refresh-token");

            // Act
            var result = await _controller.RefreshToken(refreshTokenDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeOfType<TokenResponseDto>();
        }

        [Fact]
        public async Task Logout_WithAuthenticatedUser_ShouldReturnOkResult()
        {
            // Arrange
            var user = TestDataHelper.GetTestUser();
            var claimsPrincipal = TestDataHelper.GetTestClaimsPrincipal();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Logout();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        // Helper methods to create mocks
        private Mock<UserManager<User>> GetUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                store.Object,
                null, null, null, null, null, null, null, null
            );
        }

        private Mock<RoleManager<Role>> GetRoleManagerMock()
        {
            var store = new Mock<IRoleStore<Role>>();
            return new Mock<RoleManager<Role>>(
                store.Object,
                null, null, null, null
            );
        }

        private Mock<SignInManager<User>> GetSignInManagerMock()
        {
            var userManager = GetUserManagerMock().Object;
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            
            return new Mock<SignInManager<User>>(
                userManager,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null, null, null
            );
        }
    }
}