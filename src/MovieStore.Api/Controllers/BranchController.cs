using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieStore.Api.Data;
using MovieStore.Api.MappingProfiles;
using MovieStore.Api.Middleware;
using MovieStore.Application.Interfaces;
using MovieStore.Domain;
using Newtonsoft.Json;

namespace MovieStore.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BranchController : ControllerBase
{
    private readonly ILogger<BranchController> _logger;
    private readonly IBranchService _branchService;

    public BranchController(ILogger<BranchController> logger, IBranchService branchService)
    {
        _logger = logger;
        _branchService = branchService;
    }

    [HttpPost]
    [Authorize(Roles = IdentityData.AdminUserPolicyName)]
    public async Task<IActionResult> Register([FromBody] RegisterBranchRequest model)
    {
        _logger.LogInformation($"Branch registration request received: {JsonConvert.SerializeObject(model)}");

        var result = await _branchService.Add(Mapper.Map<RegisterBranchRequest, Branch>(model));
        //var result = new { Succeeded = true, Errors = ""};
        //return BadRequest(result.Errors);

        return result is not null ? Ok() : BadRequest();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation($"Get branches request received");

        var branches = await _branchService.GetAll();

        return branches is null
        ? NotFound("No movies found")
        : Ok(branches
            .Select(Mapper.Map<Branch, UpdateBranchRequest>)
            .ToList());
    }

    // [HttpGet("{branchId}")]
    // [Authorize(Roles = IdentityData.AdminUserPolicyName)]
    // public async Task<IActionResult> Get([FromRoute] string branchId)
    // {
    //     _logger.LogInformation($"Movies request received");

    //     var branch = await _branchService.Get();

    //     if (movies is null)
    //     {
    //         return NotFound("No movies found");
    //     }

    //     var moviesDic = movies
    //         .Select(Mapper.Map<Movie, UpdateMovieRequest>)
    //         .GroupBy(movie => movie.Genre)
    //         .ToDictionary(
    //             group => group.Key,
    //             group => group.ToList());

    //     return Ok(moviesDic);
    // }

    [HttpPost("{branchId}/movie/{movieId}")]
    [Authorize(Roles = IdentityData.AdminUserPolicyName+","+IdentityData.ManagerUserPolicyName)]
    public async Task<IActionResult> AddMovie([FromRoute] string branchId, [FromRoute] string movieId)
    {
        _logger.LogInformation($"Add movie {movieId} to branch {branchId} request received");

        if (!Guid.TryParse(branchId, out var branchGuid))
        {
            return NotFound("Invalid branch Id");
        }
        if (!Guid.TryParse(movieId, out var movieGuid))
        {
            return NotFound("Invalid movie Id");
        }

        await _branchService.AddMovie(branchGuid, movieGuid);

        return Ok();
    }

    [HttpGet("{branchId}/movies")]
    public async Task<IActionResult> GetMoviesByBranch([FromRoute] string branchId)
    {
        _logger.LogInformation($"Movies by branch {branchId} request received");

        if (!Guid.TryParse(branchId, out var branchGuid))
        {
            return NotFound("Invalid branch Id");
        }

        var movies = await _branchService.GetMovies(branchGuid);
        if (movies is null)
        {
            return NotFound("Invalid branch");
        }

        var moviesDic = movies
            .Select(Mapper.Map<Movie, UpdateMovieRequest>)
            .GroupBy(movie => movie.Genre)
            .ToDictionary(
                group => group.Key,
                group => group.ToList());

        return Ok(moviesDic);
    }

    [HttpPut]
    [Authorize(Roles = IdentityData.AdminUserPolicyName+","+IdentityData.ManagerUserPolicyName)]
    public async Task<IActionResult> Put([FromBody] UpdateBranchRequest model)
    {
        _logger.LogInformation($"Branch modification request received: {JsonConvert.SerializeObject(model)}");

        var result = await _branchService.Update(Mapper.Map<UpdateBranchRequest, Branch>(model));

        //var result = new { Succeeded = true, Errors = ""};
        return result is not null ? Ok() : BadRequest();
    }

    [HttpDelete("{branchId}")]
    [Authorize(Roles = IdentityData.AdminUserPolicyName)]
    public async Task<IActionResult> Delete([FromRoute] string branchId)
    {
        _logger.LogInformation($"Delete branch {branchId} request received");

        if (!Guid.TryParse(branchId, out var branchGuid))
        {
            return NotFound("Invalid branch");
        }

        await _branchService.Delete(branchGuid);

        return Ok();
    }

    [HttpDelete("{branchId}/movie/{movieId}")]
    [Authorize(Roles = IdentityData.AdminUserPolicyName+","+IdentityData.ManagerUserPolicyName)]
    public async Task<IActionResult> DeleteMovie([FromRoute] string branchId, [FromRoute] string movieId)
    {
        _logger.LogInformation($"Remove movie {movieId} from branch {branchId} request received");

        if (!Guid.TryParse(branchId, out var branchGuid))
        {
            return NotFound("Invalid branch");
        }
        if (!Guid.TryParse(movieId, out var movieGuid))
        {
            return NotFound("Invalid movie");
        }

        await _branchService.AddMovie(branchGuid, movieGuid);

        return Ok();
    }
}