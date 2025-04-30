using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Quizzamination.Models;

namespace Quizzamination.Services
{
    public static class TestLoader
    {
        public static List<Question> LoadFromFile(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Question>>(json)!;
        }
    }
}
