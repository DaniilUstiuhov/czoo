using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AnimalManager.Resources
{
    /// <summary>
    /// Localization system with support for Estonian, Russian, and English
    /// Implements INotifyPropertyChanged for WPF data binding
    /// </summary>
    public class Localization : INotifyPropertyChanged
    {
        private static Localization _instance;
        private string _currentLanguage = "ET"; // Default: Estonian

        // Dictionary storage for all translations
        private Dictionary<string, Dictionary<string, string>> _translations;

        public event PropertyChangedEventHandler PropertyChanged;

        private Localization()
        {
            InitializeTranslations();
        }

        public static Localization Instance => _instance ??= new Localization();

        public string CurrentLanguage
        {
            get => _currentLanguage;
            private set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    OnPropertyChanged();
                    // Notify all translation keys changed
                    OnPropertyChanged("Item[]");
                }
            }
        }

        /// <summary>
        /// Indexer for accessing translations in XAML
        /// Usage: {Binding Source={x:Static res:Localization.Instance}, Path=[KeyName]}
        /// </summary>
        public string this[string key]
        {
            get
            {
                if (_translations.TryGetValue(key, out var translations))
                {
                    if (translations.TryGetValue(_currentLanguage, out var translation))
                    {
                        return translation;
                    }
                }
                return $"[{key}]"; // Return key if translation not found
            }
        }

        /// <summary>
        /// Switch to next language in cycle: ET → RU → EN → ET
        /// </summary>
        public void SwitchLanguage()
        {
            CurrentLanguage = _currentLanguage switch
            {
                "ET" => "RU",
                "RU" => "EN",
                "EN" => "ET",
                _ => "ET"
            };
        }

        /// <summary>
        /// Set specific language
        /// </summary>
        public void SetLanguage(string languageCode)
        {
            if (languageCode == "ET" || languageCode == "RU" || languageCode == "EN")
            {
                CurrentLanguage = languageCode;
            }
        }

        private void InitializeTranslations()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>
            {
                // Window and Sections
                ["WindowTitle"] = new Dictionary<string, string>
                {
                    ["ET"] = "Loomade Haldamise Süsteem",
                    ["RU"] = "Система Управления Животными",
                    ["EN"] = "Animal Management System"
                },

                ["AnimalsSection"] = new Dictionary<string, string>
                {
                    ["ET"] = "🐾 Loomad",
                    ["RU"] = "🐾 Животные",
                    ["EN"] = "🐾 Animals"
                },

                ["EnclosuresSection"] = new Dictionary<string, string>
                {
                    ["ET"] = "🏠 Voljeerid",
                    ["RU"] = "🏠 Вольеры",
                    ["EN"] = "🏠 Enclosures"
                },

                ["DetailsSection"] = new Dictionary<string, string>
                {
                    ["ET"] = "📋 Detailid",
                    ["RU"] = "📋 Детали",
                    ["EN"] = "📋 Details"
                },

                ["ActionsSection"] = new Dictionary<string, string>
                {
                    ["ET"] = "⚡ Tegevused",
                    ["RU"] = "⚡ Действия",
                    ["EN"] = "⚡ Actions"
                },

                ["LogSection"] = new Dictionary<string, string>
                {
                    ["ET"] = "📝 Logi",
                    ["RU"] = "📝 Журнал",
                    ["EN"] = "📝 Log"
                },

                ["StatisticsSection"] = new Dictionary<string, string>
                {
                    ["ET"] = "📊 LINQ Statistika",
                    ["RU"] = "📊 LINQ Статистика",
                    ["EN"] = "📊 LINQ Statistics"
                },

                // Buttons - Top Toolbar
                ["SaveLogs"] = new Dictionary<string, string>
                {
                    ["ET"] = "💾 Salvesta Logi",
                    ["RU"] = "💾 Сохранить Журнал",
                    ["EN"] = "💾 Save Logs"
                },

                ["LoadLogs"] = new Dictionary<string, string>
                {
                    ["ET"] = "📂 Lae Logi",
                    ["RU"] = "📂 Загрузить Журнал",
                    ["EN"] = "📂 Load Logs"
                },

                ["SaveToDb"] = new Dictionary<string, string>
                {
                    ["ET"] = "💾 Salvesta Andmebaasi",
                    ["RU"] = "💾 Сохранить в БД",
                    ["EN"] = "💾 Save to Database"
                },

                ["LoadFromDb"] = new Dictionary<string, string>
                {
                    ["ET"] = "📂 Lae Andmebaasist",
                    ["RU"] = "📂 Загрузить из БД",
                    ["EN"] = "📂 Load from Database"
                },

                ["SwitchLanguage"] = new Dictionary<string, string>
                {
                    ["ET"] = "🌐 Keel",
                    ["RU"] = "🌐 Язык",
                    ["EN"] = "🌐 Language"
                },

                // Buttons - Animals
                ["AddAnimal"] = new Dictionary<string, string>
                {
                    ["ET"] = "➕ Lisa Loom",
                    ["RU"] = "➕ Добавить Животное",
                    ["EN"] = "➕ Add Animal"
                },

                ["RemoveAnimal"] = new Dictionary<string, string>
                {
                    ["ET"] = "🗑️ Eemalda Loom",
                    ["RU"] = "🗑️ Удалить Животное",
                    ["EN"] = "🗑️ Remove Animal"
                },

                // Buttons - Actions
                ["MakeSound"] = new Dictionary<string, string>
                {
                    ["ET"] = "🔊 Tee Häält",
                    ["RU"] = "🔊 Издать Звук",
                    ["EN"] = "🔊 Make Sound"
                },

                ["CrazyAction"] = new Dictionary<string, string>
                {
                    ["ET"] = "🤪 Hull Tegevus",
                    ["RU"] = "🤪 Безумие",
                    ["EN"] = "🤪 Crazy Action"
                },

                ["Fly"] = new Dictionary<string, string>
                {
                    ["ET"] = "🦅 Lenda",
                    ["RU"] = "🦅 Лететь",
                    ["EN"] = "🦅 Fly"
                },

                ["FoodLabel"] = new Dictionary<string, string>
                {
                    ["ET"] = "Toit:",
                    ["RU"] = "Еда:",
                    ["EN"] = "Food:"
                },

                ["Feed"] = new Dictionary<string, string>
                {
                    ["ET"] = "🍴 Söö",
                    ["RU"] = "🍴 Кормить",
                    ["EN"] = "🍴 Feed"
                },

                ["DropFood"] = new Dictionary<string, string>
                {
                    ["ET"] = "🍖 Viska Toit Voljeeri",
                    ["RU"] = "🍖 Бросить Еду в Вольер",
                    ["EN"] = "🍖 Drop Food to Enclosure"
                },

                ["StartTimer"] = new Dictionary<string, string>
                {
                    ["ET"] = "⏰ Käivita Taimer",
                    ["RU"] = "⏰ Запустить Таймер",
                    ["EN"] = "⏰ Start Timer"
                },

                ["StopTimer"] = new Dictionary<string, string>
                {
                    ["ET"] = "⏸️ Peata",
                    ["RU"] = "⏸️ Стоп",
                    ["EN"] = "⏸️ Stop"
                },

                ["ClearLog"] = new Dictionary<string, string>
                {
                    ["ET"] = "🗑️ Tühjenda Logi",
                    ["RU"] = "🗑️ Очистить Журнал",
                    ["EN"] = "🗑️ Clear Log"
                },

                ["UpdateStats"] = new Dictionary<string, string>
                {
                    ["ET"] = "🔄 Uuenda Statistikat",
                    ["RU"] = "🔄 Обновить Статистику",
                    ["EN"] = "🔄 Update Statistics"
                },

                // Messages
                ["SelectAnimal"] = new Dictionary<string, string>
                {
                    ["ET"] = "Vali loom nimekirjast...",
                    ["RU"] = "Выберите животное из списка...",
                    ["EN"] = "Select an animal from the list..."
                },

                ["Welcome"] = new Dictionary<string, string>
                {
                    ["ET"] = "🎉 Tere tulemast loomade haldamise süsteemi!",
                    ["RU"] = "🎉 Добро пожаловать в систему управления животными!",
                    ["EN"] = "🎉 Welcome to Animal Management System!"
                },

                ["AnimalsLoaded"] = new Dictionary<string, string>
                {
                    ["ET"] = "✅ Laaditud {0} looma ja {1} voljeeri",
                    ["RU"] = "✅ Загружено {0} животных и {1} вольеров",
                    ["EN"] = "✅ Loaded {0} animals and {1} enclosures"
                },

                ["AnimalJoined"] = new Dictionary<string, string>
                {
                    ["ET"] = "🐾 {0} liitus voljeeriga '{1}'",
                    ["RU"] = "🐾 {0} присоединился к вольеру '{1}'",
                    ["EN"] = "🐾 {0} joined enclosure '{1}'"
                },

                ["FoodDropped"] = new Dictionary<string, string>
                {
                    ["ET"] = "🍖 {0} visati voljeeri '{1}'",
                    ["RU"] = "🍖 {0} брошена в вольер '{1}'",
                    ["EN"] = "🍖 {0} dropped to enclosure '{1}'"
                },

                ["EatingStarted"] = new Dictionary<string, string>
                {
                    ["ET"] = "  🍽️ {0}",
                    ["RU"] = "  🍽️ {0}",
                    ["EN"] = "  🍽️ {0}"
                },

                ["EatingFinished"] = new Dictionary<string, string>
                {
                    ["ET"] = "  ✅ {0} lõpetas söömise",
                    ["RU"] = "  ✅ {0} закончил есть",
                    ["EN"] = "  ✅ {0} finished eating"
                },

                ["AllEaten"] = new Dictionary<string, string>
                {
                    ["ET"] = "🎉 Kõik loomad voljeeris '{0}' said söödud!",
                    ["RU"] = "🎉 Все животные в вольере '{0}' накормлены!",
                    ["EN"] = "🎉 All animals in enclosure '{0}' are fed!"
                },

                ["AnimalSelected"] = new Dictionary<string, string>
                {
                    ["ET"] = "👉 Valitud: {0}",
                    ["RU"] = "👉 Выбран: {0}",
                    ["EN"] = "👉 Selected: {0}"
                },

                ["SoundMade"] = new Dictionary<string, string>
                {
                    ["ET"] = "🔊 {0} ütles: {1}",
                    ["RU"] = "🔊 {0} сказал: {1}",
                    ["EN"] = "🔊 {0} said: {1}"
                },

                ["AnimalFed"] = new Dictionary<string, string>
                {
                    ["ET"] = "🍴 {0} sai {1}",
                    ["RU"] = "🍴 {0} получил {1}",
                    ["EN"] = "🍴 {0} got {1}"
                },

                ["CrazyActionPerformed"] = new Dictionary<string, string>
                {
                    ["ET"] = "🤪 {0}",
                    ["RU"] = "🤪 {0}",
                    ["EN"] = "🤪 {0}"
                },

                ["FlyingStatus"] = new Dictionary<string, string>
                {
                    ["ET"] = "🦅 {0} nüüd {1}",
                    ["RU"] = "🦅 {0} теперь {1}",
                    ["EN"] = "🦅 {0} is now {1}"
                },

                ["Flying"] = new Dictionary<string, string>
                {
                    ["ET"] = "lendab",
                    ["RU"] = "летит",
                    ["EN"] = "flying"
                },

                ["NotFlying"] = new Dictionary<string, string>
                {
                    ["ET"] = "ei lenda",
                    ["RU"] = "не летит",
                    ["EN"] = "not flying"
                },

                ["AnimalAdded"] = new Dictionary<string, string>
                {
                    ["ET"] = "✅ Loom lisatud: {0}",
                    ["RU"] = "✅ Животное добавлено: {0}",
                    ["EN"] = "✅ Animal added: {0}"
                },

                ["AnimalRemoved"] = new Dictionary<string, string>
                {
                    ["ET"] = "🗑️ Loom eemaldatud: {0}",
                    ["RU"] = "🗑️ Животное удалено: {0}",
                    ["EN"] = "🗑️ Animal removed: {0}"
                },

                ["LogCleared"] = new Dictionary<string, string>
                {
                    ["ET"] = "🗑️ Logi tühjendatud",
                    ["RU"] = "🗑️ Журнал очищен",
                    ["EN"] = "🗑️ Log cleared"
                },

                ["LogsSaved"] = new Dictionary<string, string>
                {
                    ["ET"] = "💾 Logid salvestatud: {0}",
                    ["RU"] = "💾 Журналы сохранены: {0}",
                    ["EN"] = "💾 Logs saved: {0}"
                },

                ["LogsLoaded"] = new Dictionary<string, string>
                {
                    ["ET"] = "📂 Logid laaditud: {0}",
                    ["RU"] = "📂 Журналы загружены: {0}",
                    ["EN"] = "📂 Logs loaded: {0}"
                },

                ["DataSaved"] = new Dictionary<string, string>
                {
                    ["ET"] = "💾 Andmed salvestatud andmebaasi",
                    ["RU"] = "💾 Данные сохранены в базу данных",
                    ["EN"] = "💾 Data saved to database"
                },

                ["DataLoaded"] = new Dictionary<string, string>
                {
                    ["ET"] = "📂 Andmed laaditud andmebaasist",
                    ["RU"] = "📂 Данные загружены из базы данных",
                    ["EN"] = "📂 Data loaded from database"
                },

                // Warnings and Errors
                ["Warning"] = new Dictionary<string, string>
                {
                    ["ET"] = "Hoiatus",
                    ["RU"] = "Предупреждение",
                    ["EN"] = "Warning"
                },

                ["Error"] = new Dictionary<string, string>
                {
                    ["ET"] = "Viga",
                    ["RU"] = "Ошибка",
                    ["EN"] = "Error"
                },

                ["Success"] = new Dictionary<string, string>
                {
                    ["ET"] = "Edukas",
                    ["RU"] = "Успешно",
                    ["EN"] = "Success"
                },

                ["PleaseSelectAnimal"] = new Dictionary<string, string>
                {
                    ["ET"] = "Palun vali loom!",
                    ["RU"] = "Пожалуйста, выберите животное!",
                    ["EN"] = "Please select an animal!"
                },

                ["EnterFood"] = new Dictionary<string, string>
                {
                    ["ET"] = "Palun sisesta toit!",
                    ["RU"] = "Пожалуйста, введите еду!",
                    ["EN"] = "Please enter food!"
                },

                ["NotFlyable"] = new Dictionary<string, string>
                {
                    ["ET"] = "See loom ei saa lennata!",
                    ["RU"] = "Это животное не может летать!",
                    ["EN"] = "This animal cannot fly!"
                },

                ["NotCrazy"] = new Dictionary<string, string>
                {
                    ["ET"] = "Sellel loomal pole hullumeelset tegevust!",
                    ["RU"] = "У этого животного нет безумного действия!",
                    ["EN"] = "This animal has no crazy action!"
                },

                ["SelectEnclosure"] = new Dictionary<string, string>
                {
                    ["ET"] = "Palun vali voljeer!",
                    ["RU"] = "Пожалуйста, выберите вольер!",
                    ["EN"] = "Please select an enclosure!"
                },

                ["ConfirmRemove"] = new Dictionary<string, string>
                {
                    ["ET"] = "Kas oled kindel, et soovid looma eemaldada?",
                    ["RU"] = "Вы уверены, что хотите удалить животное?",
                    ["EN"] = "Are you sure you want to remove the animal?"
                },

                // Statistics Labels
                ["OldestAnimals"] = new Dictionary<string, string>
                {
                    ["ET"] = "🦴 Vanimad loomad:",
                    ["RU"] = "🦴 Старейшие животные:",
                    ["EN"] = "🦴 Oldest animals:"
                },

                ["YoungestAnimals"] = new Dictionary<string, string>
                {
                    ["ET"] = "🐣 Noorimad loomad:",
                    ["RU"] = "🐣 Младшие животные:",
                    ["EN"] = "🐣 Youngest animals:"
                },

                ["AnimalsByType"] = new Dictionary<string, string>
                {
                    ["ET"] = "📊 Loomad tüübi järgi:",
                    ["RU"] = "📊 Животные по типу:",
                    ["EN"] = "📊 Animals by type:"
                },

                ["AnimalsInEnclosures"] = new Dictionary<string, string>
                {
                    ["ET"] = "🏠 Loomad voljeerides:",
                    ["RU"] = "🏠 Животные в вольерах:",
                    ["EN"] = "🏠 Animals in enclosures:"
                },

                ["GeneralStats"] = new Dictionary<string, string>
                {
                    ["ET"] = "📈 Üldine statistika:",
                    ["RU"] = "📈 Общая статистика:",
                    ["EN"] = "📈 General statistics:"
                },

                ["TotalAnimals"] = new Dictionary<string, string>
                {
                    ["ET"] = "Kokku loomi: {0}",
                    ["RU"] = "Всего животных: {0}",
                    ["EN"] = "Total animals: {0}"
                },

                ["AverageAge"] = new Dictionary<string, string>
                {
                    ["ET"] = "Keskmine vanus: {0:F1} aastat",
                    ["RU"] = "Средний возраст: {0:F1} лет",
                    ["EN"] = "Average age: {0:F1} years"
                },

                ["WithCrazy"] = new Dictionary<string, string>
                {
                    ["ET"] = "Hulluga: {0}",
                    ["RU"] = "С безумием: {0}",
                    ["EN"] = "With crazy: {0}"
                },

                ["CanFly"] = new Dictionary<string, string>
                {
                    ["ET"] = "Saavad lennata: {0}",
                    ["RU"] = "Могут летать: {0}",
                    ["EN"] = "Can fly: {0}"
                }
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}