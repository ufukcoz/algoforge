using AlgoForge.Application.Leaderboard.Queries.GetLeaderboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlgoForge.API.Controllers;

[ApiController]
[Route("api/leaderboard")]
public class LeaderboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaderboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaderboardEntryDto>>> GetLeaderboard([FromQuery] int top = 50)
    {
        var result = await _mediator.Send(new GetLeaderboardQuery(top));
        return Ok(result);
    }
}
