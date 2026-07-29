using AlgoForge.Application.Profile.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlgoForge.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ProfileDto>> GetMyProfile()
    {
        var userId = GetCurrentUserId();
        var result = await _mediator.Send(new GetMyProfileQuery(userId));
        return Ok(result);
    }
}
