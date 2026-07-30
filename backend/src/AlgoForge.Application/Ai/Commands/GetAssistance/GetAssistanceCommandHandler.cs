using AlgoForge.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlgoForge.Application.Ai.Commands.GetAssistance;

public class GetAssistanceCommandHandler : IRequestHandler<GetAssistanceCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IAiAssistantService _aiService;

    public GetAssistanceCommandHandler(IApplicationDbContext context, IAiAssistantService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<string> Handle(GetAssistanceCommand request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (question is null)
            throw new InvalidOperationException("Soru bulunamadi.");

        var prompt = BuildPrompt(request, question.Title, question.Description, question.Difficulty.ToString());
        return await _aiService.GenerateAsync(prompt, cancellationToken);
    }

    private static string BuildPrompt(GetAssistanceCommand request, string title, string description, string difficulty)
    {
        var context = $"""
            Soru basligi: {title}
            Zorluk: {difficulty}
            Soru aciklamasi: {description}

            Kullanicinin {request.Language} dilinde yazdigi kod:
            ```
            {request.Code}
            ```
            """;

        // Her eylem icin ayri bir sistem talimati - AI'nin rolunu ve sinirlarini net tutuyoruz.
        // Ozellikle Hint'te DOGRUDAN COZUM VERMEMESI konusunda israrci olmak onemli,
        // yoksa ogrenme deneyimini oldurur (AlgoForge'un vizyon dokumaninda "cozumu vermeden
        // yonlendiren akilli ipuclari" olarak tanimlanan AI Mentor ozelligiyle uyumlu olmali).
        var instruction = request.Action switch
        {
            AiAssistAction.Hint =>
                "Sen bir programlama mentorusun. Kullaniciya bu soru icin KESINLIKLE tam cozumu " +
                "veya calisir kod VERME. Sadece dogru yone yonlendiren, dusunmeye tesvik eden 2-3 " +
                "cumlelik bir ipucu ver. Hangi veri yapisini veya yaklasimi dusunmesi gerektigini " +
                "ima et ama adim adim cozumu anlatma.",

            AiAssistAction.ComplexityAnalysis =>
                "Kullanicinin yazdigi kodun zaman karmasikligini (Big-O) ve alan karmasikligini " +
                "analiz et. Neden bu karmasiklikta oldugunu kisaca acikla (hangi dongu/veri yapisi " +
                "bu karmasikliga sebep oluyor). Kisa ve net ol, 4-5 cumleyi gecme.",

            AiAssistAction.ExplainBug =>
                "Kullanicinin kodunu incele ve varsa mantik hatalarini, edge-case eksiklerini veya " +
                "olasi calisma zamani hatalarini tespit et. Hatayi bul ama duzeltilmis kodu YAZMA, " +
                "sadece hatanin ne oldugunu ve neden sorun cikardigini acikla.",

            AiAssistAction.ExplainCode =>
                "Kullanicinin kodunun ne yaptigini, adim adim ama kisa ve anlasilir sekilde acikla. " +
                "Yeni baslayan birine anlatir gibi ol.",

            AiAssistAction.SuggestSolution =>
                "Kullanicinin mevcut yaklasimini degerlendir ve varsa daha verimli/temiz bir yaklasim " +
                "oner. Alternatif algoritma veya veri yapisini ismen belirt ve neden daha iyi olacagini " +
                "acikla, ama tam kod yazma - yaklasimi anlat.",

            _ => "Kullaniciya yardimci ol.",
        };

        return $"{instruction}\n\n{context}\n\nYanitini Turkce ver.";
    }
}
