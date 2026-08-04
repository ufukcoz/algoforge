using AlgoForge.Application.Questions.Commands.CreateQuestion;
using AlgoForge.Application.Questions.Commands.RunCode;
using AlgoForge.Application.Questions.Queries.GetQuestionById;
using AlgoForge.Application.Questions.Queries.GetQuestions;
using AlgoForge.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AlgoForge.API.Controllers;

[ApiController]
[Route("api/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuestionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedQuestionsDto>> GetQuestions(
        [FromQuery] Guid? categoryId,
        [FromQuery] Difficulty? difficulty,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetQuestionsQuery(categoryId, difficulty, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuestionDetailDto>> GetQuestionById(Guid id)
    {
        var result = await _mediator.Send(new GetQuestionByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    // Artik gercekten sadece Role=Admin olan kullanicilar soru ekleyebiliyor.
    // JwtService, login sirasinda ClaimTypes.Role claim'ini token'a ekliyor,
    // ASP.NET Core bunu otomatik olarak [Authorize(Roles=...)] ile eslestiriyor.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> CreateQuestion(CreateQuestionCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetQuestionById), new { id }, id);
    }

    // [Authorize] ile korunuyor: Judge0/RapidAPI ucretsiz katmani sinirli oldugu icin
    // anonim kullanicilarin bu endpoint'i suistimal etmesini engellemek gerekiyor.
    // "expensive" rate limit: dakikada 15 istekle sinirli, ucretsiz Judge0 kotasini korur.
    [HttpPost("{id:guid}/run")]
    [Authorize]
    [EnableRateLimiting("expensive")]
    public async Task<ActionResult<RunCodeResult>> RunCode(Guid id, RunCodeRequest request)
    {
        var result = await _mediator.Send(new RunCodeCommand(id, request.Language, request.SourceCode));
        return Ok(result);
    }
}

public record RunCodeRequest(string Language, string SourceCode);
