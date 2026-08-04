using AlgoForge.Application.Ai.Commands.GetAssistance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AlgoForge.API.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IMediator _mediator;

    public AiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("assist")]
    [EnableRateLimiting("expensive")]
    public async Task<ActionResult<AiAssistResponse>> Assist(AiAssistRequest request)
    {
        var response = await _mediator.Send(new GetAssistanceCommand(
            request.QuestionId,
            request.Code,
            request.Language,
            request.Action
        ));
        return Ok(new AiAssistResponse(response));
    }
}

public record AiAssistRequest(Guid QuestionId, string Code, string Language, AiAssistAction Action);
public record AiAssistResponse(string Message);
