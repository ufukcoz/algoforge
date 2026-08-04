using AlgoForge.Application.Submissions.Commands.CreateSubmission;
using AlgoForge.Application.Submissions.Queries.GetMySubmissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AlgoForge.API.Controllers;

[ApiController]
[Route("api/submissions")]
[Authorize]
public class SubmissionsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public SubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [EnableRateLimiting("expensive")]
    public async Task<ActionResult<SubmissionResultDto>> CreateSubmission(CreateSubmissionRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new CreateSubmissionCommand(userId, request.QuestionId, request.Language, request.SourceCode));
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<SubmissionSummaryDto>>> GetMySubmissions([FromQuery] Guid? questionId)
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetMySubmissionsQuery(userId, questionId));
        return Ok(result);
    }
}

public record CreateSubmissionRequest(Guid QuestionId, string Language, string SourceCode);
