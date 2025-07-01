# Quizzamination

**Quizzamination** is a desktop quiz and examination system built with Windows Presentation Foundation (WPF).
It is designed as a personal project (PET) to explore UI design, test logic, and local data handling in .NET.

The application supports various question types, provides a smooth test-taking experience, and displays a summary of results at the end.

---

## ✨ Features

* 📝 **Question Types:**

  * Single choice (radio buttons)
  * Multiple choice (checkboxes)
  * Matching pairs
  * True/False
  * Short answer

* 🔀 Automatic shuffling of answers and questions

* 🕒 Built-in timer and current question counter

* 📊 Summary window with statistics at the end of the test

* 📂 Export results to **JSON** or **plain text**

* 📁 Local file-based storage (no cloud, no database)

---

## 📄 JSON Structure

The application uses a structured JSON format to define quiz questions. Below is a valid example:

```json
[
  {
    "Text": "What is 2 + 2?",
    "Type": "SingleChoice",
    "Options": ["3", "4", "5"],
    "CorrectAnswers": [1]
  },
  {
    "Text": "Which of these numbers are even?",
    "Type": "MultipleChoice",
    "Options": ["1", "2", "3", "4", "5", "6"],
    "CorrectAnswers": [1, 3, 5]
  },
  {
    "Text": "The Earth revolves around the Sun.",
    "Type": "TrueFalse",
    "Options": ["True", "False"],
    "CorrectAnswers": [0]
  },
  {
    "Text": "What is the capital of Ukraine?",
    "Type": "ShortAnswer",
    "CorrectShortAnswer": "Kyiv"
  },
  {
    "Text": "Match the countries with their capitals:",
    "Type": "Matching",
    "MatchPairs": {
      "France": "Paris",
      "Italy": "Rome",
      "Spain": "Madrid"
    }
  }
]
```

---

## 🚀 Getting Started

This project targets **.NET 9.0** and uses **WPF (Windows-only)**.

You can open the solution in **Visual Studio 2022 or newer**, or build and run it from the command line:

```bash
# Build the application
dotnet build Quizzamination.sln

# Run the application
dotnet run --project Quizzamination/Quizzamination.csproj
```

> The application expects a JSON file with questions (e.g., `test.json`).
> Place the file in the working directory or adjust the path in `MainWindow.xaml.cs`.

---

## 🧹 Project Structure

```
/Quizzamination
├── 📁 bin/              # Build output (ignored by Git)
├── 📁 Models/           # Core data structures (Question, AnswerResult)
├── 📁 Services/         # Logic for loading and processing quizzes
├── 📁 Views/            # XAML-based windows and controls
│   └── 📁 Controls/     # User controls for each question type
├── 📄 App.xaml          # Application definition
├── 📄 App.xaml.cs       # Application logic
├── 📄 Quizzamination.csproj    # Project file
├── 📄 AssemblyInfo.cs   # Assembly metadata
└── 📄 Quizzamination.csproj.user # User-specific project settings
```

> Note: The `bin/` and `obj/` folders are generated during build and should be excluded from version control.

---

## 👤 Author

Created by **Yaremchuk "Yakudzito" Zakhar**

This project is part of a long-term learning journey and open to collaboration.

---

## 🙌 Credits

* .NET and WPF — Microsoft
* Icons and illustrations — [Flaticon](https://www.flaticon.com/) / [Material Design](https://material.io/)
* Inspiration — students who hate paper tests 😅

---

## 🛡 License

This project is licensed under the [MIT License](./LICENSE).
You may use, copy, modify, and distribute this project freely, as long as you include attribution.
