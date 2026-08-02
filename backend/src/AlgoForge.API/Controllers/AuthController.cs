using AlgoForge.Application.Auth.Commands.Login;
using AlgoForge.Application.Auth.Commands.Logout;
using AlgoForge.Application.Auth.Commands.RefreshToken;
using AlgoForge.Application.Auth.Commands.Register;
using AlgoForge.Application.Auth.Commands.ResendVerificationEmail;
using AlgoForge.Application.Auth.Commands.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlgoForge.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResult>> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login(LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResult>> Refresh(RefreshTokenRequest request)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken));
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        await _mediator.Send(new LogoutCommand(request.RefreshToken));
        return NoContent();
    }

    // Bu endpoint'e kullanici emaildeki linke tiklayarak tarayicidan ulasir,
    // o yuzden JSON degil dogrudan bir HTML sayfasi donuyoruz.
    [HttpGet("verify-email")]
    public async Task<ContentResult> VerifyEmail([FromQuery] string token)
    {
        var success = await _mediator.Send(new VerifyEmailCommand(token));

        var html = success
            ? "<html><body style='font-family:sans-serif;text-align:center;padding:60px;'>" +
              "<h2 style='color:#22C55E;'>E-posta adresin dogrulandi!</h2>" +
              "<p>Artik AlgoForge uygulamasina donup giris yapabilirsin.</p></body></html>"
            : "<html><body style='font-family:sans-serif;text-align:center;padding:60px;'>" +
              "<h2 style='color:#EF4444;'>Dogrulama basarisiz</h2>" +
              "<p>Link gecersiz veya suresi dolmus olabilir. Uygulamadan tekrar dogrulama emaili isteyebilirsin.</p></body></html>";

        return Content(html, "text/html");
    }

    [HttpPost("resend-verification")]
    [Authorize]
    public async Task<IActionResult> ResendVerification()
    {
        var userId = GetCurrentUserId();
        await _mediator.Send(new ResendVerificationEmailCommand(userId));
        return NoContent();
    }
}

public record RefreshTokenRequest(string RefreshToken);
