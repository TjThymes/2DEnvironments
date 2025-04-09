using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ObjectEnvironmentPlacer.Controllers;
using ObjectEnvironmentPlacer.Other;
using ObjectEnvironmentPlacer.Objects;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using ObjectEnvironmentPlacer.Interface;

public class AuthControllerTests
{
    private Mock<IUserRepository> _userRepoMock = new Mock<IUserRepository>();
    private Mock<UserManager<ApplicationUser>> _userManagerMock;
    private Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();


    public AuthControllerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenUserIsCreated()
    {
        // Arrange
        var controller = new AuthController(_userRepoMock.Object, _userManagerMock.Object, _jwtTokenGeneratorMock.Object);

        var registerModel = new RegisterModel
        {
            Name = "TestUser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await controller.Register(registerModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUsernameAlreadyExists()
    {
        // Arrange
        var controller = new AuthController(_userRepoMock.Object, _userManagerMock.Object, _jwtTokenGeneratorMock.Object);

        _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

        var registerModel = new RegisterModel
        {
            Name = "ExistingUser",
            Email = "existing@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await controller.Register(registerModel);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

}
