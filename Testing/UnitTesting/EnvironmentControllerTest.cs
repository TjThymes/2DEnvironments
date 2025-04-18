﻿using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ObjectEnvironmentPlacer.Controllers;
using ObjectEnvironmentPlacer.Interface;
using ObjectEnvironmentPlacer.Objects;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

public class EnvironmentControllerTests
{
    private readonly Mock<IEnvironment2DRepository> _envRepoMock = new Mock<IEnvironment2DRepository>();
    private readonly Mock<IPlayerEnvironmentRepository> _playerEnvRepoMock = new Mock<IPlayerEnvironmentRepository>();
    private readonly Mock<ILogger<EnvironmentController>> _loggerMock = new Mock<ILogger<EnvironmentController>>();
    private readonly EnvironmentController _controller;

    public EnvironmentControllerTests()
    {
        _controller = new EnvironmentController(_envRepoMock.Object, _playerEnvRepoMock.Object, _loggerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "test-user-id")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }


    [Fact]
    public async Task GetAll_ShouldReturnListOfEnvironments()
    {

        _envRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Environment2D>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<IEnumerable<Environment2D>>(okResult.Value);
    }
}
