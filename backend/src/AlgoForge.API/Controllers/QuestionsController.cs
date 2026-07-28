using AlgoForge.Application.Questions.Commands.CreateQuestion;
using AlgoForge.Application.Questions.Queries.GetQuestionById;
using AlgoForge.Application.Questions.Queries.GetQuestions;
using AlgoForge.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    // TODO: Rol tabanli yetkilendirme (Admin) eklendiginde [Authorize(Roles = "Admin")] olarak guncellenmeli.
    // Su an icin sadece login olmus herhangi bir kullanicinin soru eklemesini engellemek adina
    // [Authorize] birakildi; gercek admin kontrolu Sprint sonrasi eklenecek.
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> CreateQuestion(CreateQuestionCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetQuestionById), new { id }, id);
    }
}
