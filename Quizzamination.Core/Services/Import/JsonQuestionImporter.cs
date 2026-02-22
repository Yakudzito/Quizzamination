using Quizzamination.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Quizzamination.Services.Import
{
    public class JsonQuestionImporter : IQuestionImporter
    {
        private static readonly JsonSerializerOptions CachedOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public bool CanHandle(string firstNonEmptyLine, string? extension = null)
        {
            if (extension == ".json") return true;
            var c = firstNonEmptyLine.TrimStart().FirstOrDefault();
            return c is '{' or '[';
        }
        public List<Question> Import(Stream stream)
        {
            return JsonSerializer.Deserialize<List<Question>>(stream, CachedOptions)
                   ?? throw new InvalidDataException("Did not manage to deserialize JSON");
        }
    }
}
