using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ObjectEnvironmentPlacer.Controllers;
using ObjectEnvironmentPlacer.Interface;
using ObjectEnvironmentPlacer.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ObjectControllerTests
{
    private readonly Mock<IObjectRepository> _objectRepoMock = new Mock<IObjectRepository>();
    private readonly Mock<ILogger<ObjectController>> _loggerMock = new Mock<ILogger<ObjectController>>();
    private readonly ObjectController _controller;

    public ObjectControllerTests()
    {
        _controller = new ObjectController(_objectRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllObjects()
    {
        // Arrange
        _objectRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<GameObject2D>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<IEnumerable<GameObject2D>>(okResult.Value);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedObject()
    {
        // Arrange
        var environmentId = Guid.NewGuid();
        var newObject = new GameObject2D { EnvironmentID = environmentId };

        _objectRepoMock.Setup(r => r.InsertAsync(It.IsAny<GameObject2D>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(newObject);

        // Assert
        var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        var returnedObject = Assert.IsType<GameObject2D>(createdResult.Value);
        Assert.Equal(environmentId, returnedObject.EnvironmentID);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenObjectDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _objectRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new GameObject2D { ID = id });

        // Act
        var result = await _controller.Delete(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
