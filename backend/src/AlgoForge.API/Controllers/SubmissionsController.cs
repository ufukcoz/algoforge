using AlgoForge.Application.Submissions.Commands.CreateSubmission;
using AlgoForge.Application.Submissions.Queries.GetMySubmissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AlgoForge.API.Controllers;

[ApiController]
[Route("api/submissions")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
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

    // JWT'deki "sub" claim'i, kullanilan token handler'a gore ClaimTypes.NameIdentifier'a
    // otomatik map edilebilir ya da edilmeyebilir; ikisini de kontrol ederek garantiye aliyoruz.
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Token icinde gecerli bir kullanici kimligi bulunamadi.");

        return userId;
    }
}

public record CreateSubmissionRequest(Guid QuestionId, string Language, string SourceCode);
