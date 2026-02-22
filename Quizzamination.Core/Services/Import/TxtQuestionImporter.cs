using Quizzamination.Models;
using System.IO;
using System.Text;

namespace Quizzamination.Services.Import
{
    public class TxtQuestionImporter : IQuestionImporter
    {
        private enum State { Idle, InBody }

        #region Helpers
        // extracts question text from a line like -"..."
        private static string ExtractQuestionText(string line)
        {
            // line must start with -" and end with "
            if (line.Length < 3 || line[0] != '-' || line[1] != '"' || line[^1] != '"')
                throw new InvalidDataException($"Bad question header: {line}");

            return line.Substring(2, line.Length - 3);
        }
        // finds index of first unescaped '>' or -1 if none found. If more than one found, throws
        private static int FindFirstUnescapedGt(string s)
        {
            if (s is null) throw new ArgumentNullException(nameof(s));

            int first = -1;
            int backslashes = 0; // count of consecutive '\'

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                if (c == '\\')
                {
                    backslashes++;
                    continue;
                }

                if (c == '>')
                {
                    // odd number of '\' before '>' => escaped
                    if ((backslashes & 1) == 0)
                    {
                        if (first != -1)
                            throw new InvalidDataException($"Line cannot contain more than one unescaped '>': {s}");
                        first = i;
                    }
                }

                // any character other than '\' resets the counter
                backslashes = 0;
            }

            return first; // -1, if not found
        }

        // true, if the string is marked suffix with " (x)"
        private static bool IsMarked(string? s) => s?.EndsWith(" (x)") ?? throw new ArgumentNullException(nameof(s));

        // removes the " (x)" suffix if present
        private static string StripMark(string s) => s is null ? throw new ArgumentNullException(nameof(s)) : IsMarked(s) ? s[..^4] : s;

        // in order: \\ -> \, \" -> ", \> -> >.
        // Converts escaped characters to their literal counterparts.
        private static string Unescape(string s)
        {
            ArgumentNullException.ThrowIfNull(s);

            var escapedStr = new StringBuilder();

            bool isBackslashFound = false;

            foreach (var c in s)
            {
                if (c == '\\') // found a backslash
                {
                    if (isBackslashFound) // found two backslashes in a row
                    {
                        escapedStr.Append('\\');
                        isBackslashFound = false; // reset
                    }
                    else
                    {
                        isBackslashFound = true; // mark that we found a backslash
                    }
                }
                else if (isBackslashFound) // found a character after a backslash
                {
                    switch (c)
                    {
                        case '\\': escapedStr.Append('\\'); break;
                        case '\"': escapedStr.Append('\"'); break;
                        case '>': escapedStr.Append('>'); break;
                        default: escapedStr.Append('\\').Append(c); break; // keep the backslash if not a recognized escape sequence
                    }
                    isBackslashFound = false;
                }
                else // normal character
                {
                    escapedStr.Append(c);
                }
            }

            if (isBackslashFound) // trailing backslash
            {
                escapedStr.Append('\\');
            }

            return escapedStr.ToString();
        }
        // cleans the string from weird spaces and trims it
        private static string CleanStr(string s) => s.Replace('\u00A0', ' ')
                .Replace('\u202F', ' ')
                .Replace('\u2009', ' ')
                .Replace('\u200B', ' ')
                .Trim().Normalize(NormalizationForm.FormC);
        #endregion

        public bool CanHandle(string firstNonEmptyLine, string? extension = null)
        {
            if (extension == ".txt") return true;
            return firstNonEmptyLine.StartsWith("-\"");
        }
        void FinishBlock(string questionText, List<string> bodyLines, List<Question> result)
        {

            // === Filtering and cleaning the body lines ===
            if (string.IsNullOrWhiteSpace(questionText))
                throw new InvalidDataException("Questions can not be empty.");

            for (int i = 0; i < bodyLines.Count; i++)
            {
                bodyLines[i] = CleanStr(bodyLines[i]);
            }

            questionText = CleanStr(questionText);

            bodyLines.RemoveAll(string.IsNullOrWhiteSpace);

            if (bodyLines.Count == 0)
                throw new InvalidDataException($"Question without a body: {questionText}");

            // === Question type detection and creation ===

            // Mixed types are not allowed, so we check for each type in order and if matched, create the question and return.
            bool hasArrowsInEveryLine = bodyLines.All(line => FindFirstUnescapedGt(line) > 0);
            bool hasMark = bodyLines.Any(IsMarked);
            if (hasArrowsInEveryLine && hasMark)
                throw new InvalidDataException($"Mixed question types are not allowed (Matching and SC/MC): {questionText}");

            // TrueFalse
            if (bodyLines.Count == 1 && bool.TryParse(bodyLines[0], out bool tf))
            {
                result.Add(new Question
                {
                    Text = questionText,
                    Type = QuestionType.TrueFalse,
                    CorrectAnswers = [tf ? 1 : 0]
                });
            }
            // Matching
            else if (hasArrowsInEveryLine)
            {
                var leftSet = new HashSet<string>();
                var rightSet = new HashSet<string>();
                var pairs = new Dictionary<string, string>();
                foreach (var line in bodyLines)
                {
                    int idx = FindFirstUnescapedGt(line);
                    if (idx < 0)
                        throw new InvalidDataException($"Inconsistent Matching question body: {questionText}");
                    var left = CleanStr(Unescape(line[..idx]));
                    var right = CleanStr(Unescape(line[(idx + 1)..]));
                    if (!leftSet.Add(left))
                        throw new InvalidDataException($"Duplicate left item in Matching question: {left} in question {questionText}");
                    if (!rightSet.Add(right))
                        throw new InvalidDataException($"Duplicate right item in Matching question: {right} in question {questionText}");
                    pairs[left] = right;
                }
                result.Add(new Question
                {
                    Text = questionText,
                    Type = QuestionType.Matching,
                    MatchPairs = pairs
                });
            }
            // SC / MC
            else if (hasMark)
            {
                int markedCount = bodyLines.Count(IsMarked);
                List<string> options = [];
                List<int> correctAnswers = [];
                for (int i = 0; i < bodyLines.Count; i++)
                {
                    string text;
                    if (IsMarked(bodyLines[i]))
                    {
                        text = StripMark(bodyLines[i]);
                        correctAnswers.Add(i);
                    }
                    else
                    {
                        text = bodyLines[i];
                    }
                    text = Unescape(text).TrimEnd();
                    if (string.IsNullOrEmpty(text)) throw new InvalidDataException($"Empty option in question: {questionText}");
                    options.Add(text);
                }

                result.Add(new Question
                {
                    Text = questionText,
                    Type = markedCount == 1 ? QuestionType.SingleChoice : QuestionType.MultipleChoice,
                    Options = options,
                    CorrectAnswers = correctAnswers
                });
            }
            // ShortAnswer
            else
            {
                string answer = Unescape(bodyLines[0]).Trim();
                if (string.IsNullOrEmpty(answer))
                    throw new InvalidDataException($"Empty ShortAnswer in question: {questionText}");

                result.Add(new Question
                {
                    Text = questionText,
                    Type = QuestionType.ShortAnswer,
                    CorrectShortAnswer = answer
                });
            }

        }
        public List<Question> Import(Stream stream)
        {
            var result = new List<Question>();

            string? currentQuestionText = null;
            var bodyLines = new List<string>();

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream, Encoding.UTF8, true);

            var state = State.Idle;

            int lineNo = 0;
            int headerLineNo = 0;

            while (reader.ReadLine() is { } line)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("-\""))
                {
                    if (state == State.InBody)
                    {
                        try { FinishBlock(currentQuestionText!, bodyLines, result); }
                        catch (Exception ex)
                        {
                            throw new InvalidDataException(
                                $"Q#{result.Count + 1} (header at line {headerLineNo}): {ex.Message}");
                        }
                        bodyLines.Clear();
                    }

                    currentQuestionText = ExtractQuestionText(line);
                    headerLineNo = lineNo;
                    state = State.InBody;
                    continue;
                }

                if (state != State.InBody)
                    throw new InvalidDataException($"Body without a title at line {lineNo}: {line}");

                bodyLines.Add(line);
            }

            // EOF
            if (state == State.InBody)
            {
                try { FinishBlock(currentQuestionText!, bodyLines, result); }
                catch (Exception ex)
                {
                    throw new InvalidDataException(
                        $"Q#{result.Count + 1} (header at line {headerLineNo}): {ex.Message}");
                }
            }
            if (result.Count == 0)
                throw new InvalidDataException("No questions found.");

            return result;
        }
    }
}
