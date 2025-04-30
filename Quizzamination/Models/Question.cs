using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quizzamination.Models
{
    public enum QuestionType
    {
        SingleChoice,
        MultipleChoice,
        Matching,
        ShortAnswer,
        TrueFalse
    }
    public class Question
    {
        public string Text { get; set; } = null!;
        public QuestionType Type { get; set; }
        public List<string>? Options { get; set; }
        public List<int>? CorrectAnswers { get; set; }
        public Dictionary<string, string>? MatchPairs { get; set; }
        public string? CorrectShortAnswer { get; set; }
    }
}
