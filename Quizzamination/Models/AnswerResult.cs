using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quizzamination.Models
{
    public class AnswerResult
    {
        public Question Question { get; set; }
        public object? UserAnswer { get; set; }
        public bool IsCorrect { get; set; }

        public AnswerResult(Question question, object? userAnswer, bool isCorrect)
        {
            Question = question;
            UserAnswer = userAnswer;
            IsCorrect = isCorrect;
        }
    }
}