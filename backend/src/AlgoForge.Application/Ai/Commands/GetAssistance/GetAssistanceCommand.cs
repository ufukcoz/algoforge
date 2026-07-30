using MediatR;

namespace AlgoForge.Application.Ai.Commands.GetAssistance;

public enum AiAssistAction
{
    Hint,               // cozumu vermeden yonlendirici ipucu
    ComplexityAnalysis, // zaman/alan karmasikligi analizi
    ExplainBug,         // kodda olasi hatalari acikla
    ExplainCode,        // kodun ne yaptigini acikla
    SuggestSolution,    // daha iyi bir yaklasim oner
}

public record GetAssistanceCommand(
    Guid QuestionId,
    string Code,
    string Language,
    AiAssistAction Action
) : IRequest<string>;
