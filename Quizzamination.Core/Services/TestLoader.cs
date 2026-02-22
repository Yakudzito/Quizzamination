using System.IO;
using Quizzamination.Models;
using Quizzamination.Services.Import;

namespace Quizzamination.Services
{

    public static class TestLoader
    {
        public static List<Question> LoadFromFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            var firstLine = ReadFirstNonEmptyLine(filePath);

            var importers = new List<IQuestionImporter>
            {
                new JsonQuestionImporter(),
                new TxtQuestionImporter()
            };

            foreach (var importer in importers)
            {
                if (importer.CanHandle(firstLine, extension))
                {
                    using var stream = File.OpenRead(filePath);
                    return importer.Import(stream);
                }
            }

            throw new NotSupportedException($"Невідомий формат файлу: {filePath}");
        }

        private static string ReadFirstNonEmptyLine(string filePath)
        {
            using var reader = new StreamReader(filePath);
            while (reader.ReadLine() is { } line)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    return line.Trim();
            }
            return string.Empty;
        }
        
        
    }
}
