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

            QuestionType.TrueFalse => userAnswer is int tf
                                      && q.CorrectAnswers is { Count: 1 }
                                      && tf == q.CorrectAnswers[0],

            QuestionType.MultipleChoice => userAnswer is List<int> ua
                                           && q.CorrectAnswers is { } ca
                                           && ua.OrderBy(x => x).SequenceEqual(ca.OrderBy(x => x)),

            QuestionType.ShortAnswer => string.Equals(
                (q.CorrectShortAnswer ?? "").Trim(),
                (userAnswer as string ?? "").Trim(),
                System.StringComparison.OrdinalIgnoreCase),

            QuestionType.Matching => userAnswer is Dictionary<string, string> map
                                     && q.MatchPairs is { } pairs
                                     && pairs.All(kv => map.TryGetValue(kv.Key, out var v)
                                                        && string.Equals(v?.Trim(), kv.Value.Trim(),
                                                            System.StringComparison.OrdinalIgnoreCase)),

            _ => false
        };
    }
}