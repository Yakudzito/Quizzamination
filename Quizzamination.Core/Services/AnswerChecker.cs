using Quizzamination.Models;

namespace Quizzamination.Services;

public class AnswerChecker
{
    public static bool IsCorrect(Question q, object? userAnswer)
    {
        return q.Type switch
        {
            QuestionType.SingleChoice => userAnswer is int i
                                         && q.CorrectAnswers is { Count: 1 }
                                         && i == q.CorrectAnswers[0],

            QuestionType.ShortAnswer => string.Equals(
                (q.CorrectShortAnswer ?? "").Trim(),
                (userAnswer as string ?? "").Trim(),
                StringComparison.InvariantCultureIgnoreCase),

            _ => false
        };
    }
}