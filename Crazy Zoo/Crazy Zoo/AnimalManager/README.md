AnimalManager is a professional zoo management system built on WPF (.NET 8.0) with multilingual support (Estonian, Russian, English), animal and enclosure management, logging, and integration with SQL Server LocalDB.

📋 Contents

Features
Technology
Quick Start
Architecture
Project Structure
Usage
Database
Localization
Development

✨ Features
🦁 Animal Management

Add, delete, and view animals
5 types: cats, dogs, birds, raccoons, monkeys
Unique actions for each type
Feeding with history
Detailed information

🏠 Enclosure Management

Automatic enclosure creation
Up to 5 animals of the same type
Occupancy tracking
Group feeding

📊 LINQ statistics

Statistics by type (number, average age)
Animal grouping (GroupBy)
Enclosure assignment
Action counters

🌍 Multilingualism

Estonian (ET), Russian (RU), English (EN)
Dynamic switching
Full UI localization

📝 Logging

XML and JSON Formats
Timestamps
Saving/Loading

🗄️ Database

SQL Server LocalDB
Automatic Structure Creation
CRUD Operations
State Saving

⏰ Events

Day/Night Timer
Thread-Safe Processing
UI Notifications

🛠️ Technologies

.NET 8.0 + C# 12.0
WPF + XAML
SQL Server LocalDB
Microsoft.Data.SqlClient 5.2.0
ADO.NET

Patterns

MVVM
Repository
Singleton (Localization)
Strategy (Logging)
Observer (Events)

🚀 Quick Start
Requirements

Windows 10/11 (64-bit)
.NET 8.0 SDK
Visual Studio 2022 / VS Code
SQL Server LocalDB

Installation
bashgit clone https://github.com/yourusername/AnimalManager.git
cd AnimalManager
dotnet restore
dotnet build
dotnet run

🏗️ Architecture
Presentation Layer (WPF)
↓
Business Logic Layer
↓
Data Access Layer
↓
Data Storage (LocalDB)
Components
Presentation:

MainWindow.xaml
AddAnimalDialog.xaml
Localization.cs

Business Logic:

Animal (abstract base)
Cat, Dog, Bird, Raccoon, Monkey
Enclosure<T>
ZooEventManager

Data Access:

IAnimalRepository
LocalDbAnimalRepository
ILogger (XmlLogger, JsonLogger)

📁 Project Structure
AnimalManager/
├── Models/
│ ├── Animal.cs
│ ├── Cat.cs, Dog.cs, Bird.cs
│ ├── Raccoon.cs, Monkey.cs
│ └── Enclosure.cs
│
├── Repositories/
│ ├── IRepository.cs
│ ├── IAnimalRepository.cs
│ └── LocalDbAnimalRepository.cs
│
├── Services/
│ ├── ILogger.cs
│ ├── XmlLogger.cs, JsonLogger.cs
│ └── ZooEventManager.cs
│
├── Resources/
│ └── Localization.cs
│
├── Views/
│ ├── MainWindow.xaml(.cs)
│ └── AddAnimalDialog.xaml(.cs)
│
└── App.xaml(.cs)

📖 Usage
Adding an animal

"Lisa loom" → Enter data → "Lisa"

Removing

Select an animal → "Eemalda loom"

Actions

"Tee hääl" - Sound
"Crazy Action!" - Special Action
"Lenda" - Flight (birds)
"Toida" - Feed

Change Language
🌐 ET/RU/EN → ET → RU → EN → ET
Database

"Salvesta Andmebaasi" - Save
"Lae Andmebaasi" - Load

Logs

"Salvesta Logi" - Save (XML/JSON)
"Lae Logi" - Load

Statistics
"Uuenda statistikat" - Refresh

🗄️ Database
Path: %APPDATA%\AnimalManager\AnimalManager.mdf
Schema
Animals:
sqlId INT IDENTITY PRIMARY KEY,
Name NVARCHAR(100),
Type NVARCHAR(50),
Age INT,
ExtraInfo NVARCHAR(500),
EnclosureId NVARCHAR(100)
Enclosures:
sqlId INT IDENTITY PRIMARY KEY,
Name NVARCHAR(100) UNIQUE,
Capacity INT DEFAULT 5

🌍 Localization
LanguageCodeStatusEstonianET✅ PrimaryRussianRU✅ FullEnglishEN✅ Full
Adding a language
Resources/Localization.cs:
csharpprivate void LoadFRTranslations()
{
_translations["FR"] = new Dictionary<string, string>
{
["AnimalsSection"] = "Animaux",
// ...
};
}

👨‍💻 Development
Build
bash# Debug
dotnet build --configuration Debug

# Release
dotnet build --configuration Release

# Run
dotnet run
Adding an animal type
csharppublic class Tiger : Animal
{
public override string Type => "Tiger";

public Tiger(string name, int age, string extraInfo = "")
: base(name, age, extraInfo) { }

public override string MakeSound()
{
return $"{Name}: ROARRR! 🐯";
}

public override string PerformCrazyAction()
{
CrazyActionCount++;
return $"{Name} shows its fangs!";
}
}
Add to AddAnimalDialog.xaml.cs:
csharpanimal = animalType switch
{ 
// ... existing types ... 
"Tiger" => new Tiger(name, age, extraInfo), 
_ => null
};
Code Style

PascalCase: classes, methods, properties
camelCase: variables, parameters
_camelCase: private fields
UPPER_CASE: constants

🧪 Testing
Checklist

Add/remove animals of all types
Feeding, sounds, crazy actions
Bird flight
Aviaries (creation, filling)
Timer (start/stop)
Languages ​​(ET/RU/EN)
Database saving/loading
Log saving/loading (XML/JSON)
Statistics
Log clearing

🐛 Roadmap

Unit tests
PostgreSQL/MySQL support
Excel export
Animal photos
Feeding schedule
Medical records
Reports and analytics
Multi-platform (.NET MAUI)

📄 License
MIT License
Copyright (c) 2024 AnimalManager Contributors

Permission