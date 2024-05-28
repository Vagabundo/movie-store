using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MovieStore.Application.Interfaces;
using MovieStore.Application.Services;
using MovieStore.Domain;

namespace MovieStore.Application.Tests;

public class BranchServiceTests
{
    private BranchService _service;
    private Mock<ILogger<BranchService>> _loggerMock;
    private Mock<IBranchRepository> _branchRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<BranchService>>();
        _branchRepositoryMock = new Mock<IBranchRepository>();
        _service = new BranchService(_loggerMock.Object, _branchRepositoryMock.Object);
    }

    #region Add
    [Test]
    public async Task Add_WhenUserIdIsEmpty_ShouldReturnNull()
    {
        Branch invalidBranch = GetValidBranch();
        invalidBranch.UserId = Guid.Empty;

        var result = await _service.Add(invalidBranch);

        result.Should().BeNull();
        _branchRepositoryMock.Verify(x => x.Add(It.IsAny<Branch>()), Times.Never);
    }

    [Test]
    public async Task Add_WhenAddressIsEmpty_ShouldReturnNull()
    {
        Branch invalidBranch = GetValidBranch();
        invalidBranch.Address = "";

        var result = await _service.Add(invalidBranch);

        result.Should().BeNull();
        _branchRepositoryMock.Verify(x => x.Add(It.IsAny<Branch>()), Times.Never);
    }

    [Test]
    public async Task Add_WhenCityIsEmpty_ShouldReturnNull()
    {
        Branch invalidBranch = GetValidBranch();
        invalidBranch.City = "";

        var result = await _service.Add(invalidBranch);

        result.Should().BeNull();
        _branchRepositoryMock.Verify(x => x.Add(It.IsAny<Branch>()), Times.Never);
    }

    [Test]
    public async Task Add_WhenCountryIsEmpty_ShouldReturnNull()
    {
        Branch invalidBranch = GetValidBranch();
        invalidBranch.Country = "";

        var result = await _service.Add(invalidBranch);

        result.Should().BeNull();
        _branchRepositoryMock.Verify(x => x.Add(It.IsAny<Branch>()), Times.Never);
    }

    [Test]
    public async Task Add_WhenPostalCodeIsEmpty_ShouldReturnNull()
    {
        Branch invalidBranch = GetValidBranch();
        invalidBranch.PostalCode = "";

        var result = await _service.Add(invalidBranch);

        result.Should().BeNull();
        _branchRepositoryMock.Verify(x => x.Add(It.IsAny<Branch>()), Times.Never);
    }

    [Test]
    public async Task Add_WhenEntityIsValid_ShouldReturnEntityGivenFromDB()
    {
        Branch validBranch = GetValidBranch();

        _branchRepositoryMock.Setup(x => x.Add(validBranch)).ReturnsAsync(validBranch);
        var result = await _service.Add(validBranch);

        result.Should().NotBeNull();
        result?.Equals(validBranch);
        _branchRepositoryMock.VerifyAll();
    }

    #endregion Add

    #region AddMovie

    [Test]
    public async Task AddMovie_WhenBranchIdIsNull_ReturnsNull()
    {
        Guid movieId = Guid.NewGuid();
        Guid branchId = Guid.Empty;

        var result = await _service.AddMovie(branchId, movieId);

        result.Should().BeNull();
        _branchRepositoryMock.Verify(x => x.AddMovie(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task AddMovie_WhenMovieIdIsNull_ReturnsNull()
    {
        Guid movieId = Guid.Empty;
        Guid branchId = Guid.NewGuid();

        var result = await _service.AddMovie(branchId, movieId);

        result.Should().BeNull();
        _branchRepositoryMock.Verify(x => x.AddMovie(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task AddMovie_WhenEntityIsValid_ShouldReturnEntityGivenFromDB()
    {
        BranchMovie branchMovie = new ()
        {
            BranchId = Guid.NewGuid(),
            MovieId = Guid.NewGuid()
        };

        _branchRepositoryMock.Setup(x => x.AddMovie(branchMovie.BranchId, branchMovie.MovieId)).ReturnsAsync(branchMovie);
        var result = await _service.AddMovie(branchMovie.BranchId, branchMovie.MovieId);

        result.Should().NotBeNull();
        result?.Equals(branchMovie);
        _branchRepositoryMock.VerifyAll();
    }

    #endregion AddMovie

    private Branch GetValidBranch()
    {
        return new()
        {
            UserId = Guid.NewGuid(),
            Address = "test address",
            City = "test city",
            Country = "test country",
            PostalCode = "test PC"
        };
    }
}