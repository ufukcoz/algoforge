namespace AlgoForge.Application.Common.Interfaces;

// Bu arayuz sayesinde Gemini'den baska bir LLM saglayicisina (OpenAI, Claude, Groq vb.)
// gecmek istersek sadece Infrastructure'daki implementasyonu degistirmemiz yeterli.
public interface IAiAssistantService
{
    Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken);
}
