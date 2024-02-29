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
public class MovieController : ControllerBase
{
    private readonly ILogger<MovieController> _logger;
    private readonly IMovieService _movieService;

    public MovieController(ILogger<MovieController> logger, IMovieService movieService)
    {
        _logger = logger;
        _movieService = movieService;
    }

    [HttpPost("register")]
    [Authorize(Roles = IdentityData.ManagerUserPolicyName)]
    public async Task<IActionResult> Register([FromBody] RegisterMovieRequest model)
    {
        _logger.LogInformation($"Movie registration request received: {JsonConvert.SerializeObject(model)}");

        var result = await _movieService.Add(Mapper.Map<RegisterMovieRequest, Movie>(model));
        //var result = new { Succeeded = true, Errors = ""};
        if (result is not null)
        {
            return Ok();
        }
        else
        {
            //return BadRequest(result.Errors);
            return BadRequest();
        }
    }

    [HttpPut("modify")]
    [Authorize(Roles = IdentityData.ManagerUserPolicyName)]
    public async Task<IActionResult> Modify([FromBody] ModifyMovieRequest model)
    {
        _logger.LogInformation($"Movie modification request received: {JsonConvert.SerializeObject(model)}");

        var result = await _movieService.Modify(Mapper.Map<ModifyMovieRequest, Movie>(model));
        //var result = new { Succeeded = true, Errors = ""};
        if (result is not null)
        {
            return Ok();
        }
        else
        {
            //return BadRequest(result.Errors);
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation($"Movies request received");

        var movies = await _movieService.GetAll();
        //var movies = new List<Movie>();

        if (movies is null)
        {
            return NotFound("No movies found");
        }

        var moviesDic = movies
            .Select(Mapper.Map<Movie, ModifyMovieRequest>)
            .GroupBy(movie => movie.Genre)
            .ToDictionary(
                group => group.Key,
                group => group.ToList());

        return Ok(moviesDic);
    }

    [HttpGet("moviesbybranch/{branchId}")]
    public async Task<IActionResult> GetMoviesByBranch([FromRoute] string branchId)
    {
        _logger.LogInformation($"Movies by branch {branchId} request received");

        Guid branchGuid;
        if (!Guid.TryParse(branchId, out branchGuid))
        {
            return NotFound("Invalid branch");
        }
        //var movies = await _movieService.GetByBranch(branchGuid);
        var movies = new List<Movie>();

        if (movies is null)
        {
            return NotFound("No movies found");
        }

        var moviesDic = movies
            .Select(Mapper.Map<Movie, ModifyMovieRequest>)
            .GroupBy(movie => movie.Genre)
            .ToDictionary(
                group => group.Key,
                group => group.ToList());

        return Ok(moviesDic);
    }
}