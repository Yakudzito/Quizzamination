using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Quizzamination.Models;

namespace Quizzamination.Services
{
    public static class TestLoader
    {
        public static List<Question> LoadFromFile(string filePath)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            using var streamReader = File.OpenRead(filePath);
            return JsonSerializer.Deserialize<List<Question>>(streamReader, options)!;
        }
    }
}
