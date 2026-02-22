using Quizzamination.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quizzamination.Services
{
    public interface IQuestionImporter
    {
        bool CanHandle(string firstNonEmptyLine, string? extension = null);
        List<Question> Import(Stream stream);
    }
}
