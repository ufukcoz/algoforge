using AlgoForge.Application.Contests.Commands.CreateContest;
using AlgoForge.Application.Contests.Commands.JoinContest;
using AlgoForge.Application.Contests.Queries.GetContestById;
using AlgoForge.Application.Contests.Queries.GetContestLeaderboard;
using AlgoForge.Application.Contests.Queries.GetContests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlgoForge.API.Controllers;

[ApiController]
[Route("api/contests")]
[Authorize]
public class ContestsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ContestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ContestSummaryDto>>> GetContests()
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetContestsQuery(userId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContestDetailDto>> GetContestById(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetContestByIdQuery(id, userId));
        return result is null ? NotFound() : Ok(result);
    }

    // TODO: Rol tabanli yetkilendirme (Admin/Egitmen) eklendiginde kisitlanabilir.
    // Su an herhangi bir kullanici yarisma olusturabiliyor - "Universite Modu" (roadmap)
    // geldiginde bu ogretim elemanlarina ozel bir yetkiye baglanacak.
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateContest(CreateContestRequest request)
    {
        var userId = GetCurrentUserId();
        var id = await _mediator.Send(new CreateContestCommand(
            userId,
            request.Title,
            request.Description,
            request.StartTime,
            request.EndTime,
            request.IsPublic,
            request.Questions
        ));
        return CreatedAtAction(nameof(GetContestById), new { id }, id);
    }

    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> JoinContest(Guid id, JoinContestRequest request)
    {
        var userId = GetCurrentUserId();
        await _mediator.Send(new JoinContestCommand(userId, id, request.InviteCode));
        return NoContent();
    }

    [HttpGet("{id:guid}/leaderboard")]
    public async Task<ActionResult<List<ContestLeaderboardEntryDto>>> GetLeaderboard(Guid id)
    {
        var result = await _mediator.Send(new GetContestLeaderboardQuery(id));
        return Ok(result);
    }
}

public record CreateContestRequest(
    string Title,
    string Description,
    DateTime StartTime,
    DateTime EndTime,
    bool IsPublic,
    List<ContestQuestionInput> Questions
);

public record JoinContestRequest(string? InviteCode);
