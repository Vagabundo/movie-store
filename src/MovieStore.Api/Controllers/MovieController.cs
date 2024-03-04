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
        //return BadRequest(result.Errors);

        return result is null ? BadRequest() : Ok();
    }

    [HttpPut]
    [Authorize(Roles = IdentityData.ManagerUserPolicyName)]
    public async Task<IActionResult> Update([FromBody] UpdateMovieRequest model)
    {
        _logger.LogInformation($"Movie modification request received: {JsonConvert.SerializeObject(model)}");

        var result = await _movieService.Update(Mapper.Map<UpdateMovieRequest, Movie>(model));

        return result is null ? BadRequest() : Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation($"Movies request received");

        var movies = await _movieService.GetAll();

        return movies is null
        ? NotFound("No movies found")
        : Ok(movies
            .Select(Mapper.Map<Movie, UpdateMovieRequest>)
            .GroupBy(movie => movie.Genre)
            .ToDictionary(
                group => group.Key,
                group => group.ToList()));
    }
}