using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AnimalManager.Repositories;
using AnimalManager.Services;

namespace AnimalManager
{
    public partial class MainWindow : Window
    {
        // НОВОЕ: Repository вместо просто коллекции
        private readonly IRepository<Animal> _animalRepository;

        private ObservableCollection<Animal> animals;
        private ObservableCollection<string> logEntries;
        private ObservableCollection<string> statistics;  // НОВОЕ: LINQ статистика

        // НОВОЕ: Вольеры (Enclosures)
        private ObservableCollection<Enclosure<Animal>> enclosures;

        // НОВОЕ: Таймер событий
        private readonly ZooEventManager _eventManager;

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация Repository
            _animalRepository = new InMemoryRepository<Animal>();

            animals = new ObservableCollection<Animal>();
            logEntries = new ObservableCollection<string>();
            statistics = new ObservableCollection<string>();
            enclosures = new ObservableCollection<Enclosure<Animal>>();

            // Таймер (каждые 10 секунд)
            _eventManager = new ZooEventManager(10);
            _eventManager.LogMessage += (s, msg) => AddToLog(msg);
            _eventManager.NightEvent += (s, e) => AddToLog($"🌙 {e.Message}");

            // Привязка к UI
            AnimalsListBox.ItemsSource = animals;
            LogListBox.ItemsSource = logEntries;
            StatisticsListBox.ItemsSource = statistics;
            EnclosuresListBox.ItemsSource = enclosures;

            SetupKeyboardShortcuts();
            InitializeData();
        }

        private void InitializeData()
        {
            // Создаём вольеры
            var enclosure1 = new Enclosure<Animal>("Voljeer A", 5);
            var enclosure2 = new Enclosure<Animal>("Voljeer B", 5);
            var enclosure3 = new Enclosure<Animal>("Voljeer C", 3);

            // Подписываемся на события вольеров
            enclosure1.AnimalJoinedInSameEnclosure += OnAnimalJoinedEnclosure;
            enclosure1.FoodDropped += OnFoodDroppedInEnclosure;
            enclosure2.AnimalJoinedInSameEnclosure += OnAnimalJoinedEnclosure;
            enclosure2.FoodDropped += OnFoodDroppedInEnclosure;
            enclosure3.AnimalJoinedInSameEnclosure += OnAnimalJoinedEnclosure;
            enclosure3.FoodDropped += OnFoodDroppedInEnclosure;

            enclosures.Add(enclosure1);
            enclosures.Add(enclosure2);
            enclosures.Add(enclosure3);

            // Создаём животных
            var animalsList = new Animal[]
            {
                new Cat("Muri", 3, "juustu"),
                new Dog("Rex", 5, "Saksa lambakoer"),
                new Bird("Piip", 2, "kollane"),
                new Raccoon("Riku", 4),
                new Monkey("Mango", 6),
                new Cat("Miisu", 4, "kala"),
                new Dog("Bobik", 3, "Koerapoeg")
            };

            // Добавляем в Repository и UI
            foreach (var animal in animalsList)
            {
                _animalRepository.Add(animal);
                animals.Add(animal);
            }

            // Распределяем по вольерам
            enclosure1.AddAnimal(animalsList[0]);
            enclosure1.AddAnimal(animalsList[1]);
            enclosure2.AddAnimal(animalsList[2]);
            enclosure2.AddAnimal(animalsList[3]);
            enclosure3.AddAnimal(animalsList[4]);

            AddToLog("🎉 Tere tulemast loomade haldamise süsteemi!");
            AddToLog($"📊 Laaditud {animals.Count} looma ja {enclosures.Count} voljeerid.");

            UpdateStatistics();
        }

        // ========== EVENT HANDLERS ==========

        private void OnAnimalJoinedEnclosure(object sender, AnimalEventArgs e)
        {
            var enclosure = sender as Enclosure<Animal>;
            AddToLog($"➕ {e.Animal.Name} liitus voljeeri {e.EnclosureName}!");

            // Все соседи реагируют
            foreach (var animal in enclosure.Animals.Where(a => a != e.Animal))
            {
                AddToLog($"  💬 {animal.ReactToNewNeighbor(e.Animal)}");
            }
        }

        private async void OnFoodDroppedInEnclosure(object sender, FoodEventArgs e)
        {
            var enclosure = sender as Enclosure<Animal>;
            AddToLog($"🍖 Toit ({e.FoodType}) visati voljeeri {e.EnclosureName}!");

            // Животные едят ПОСЛЕДОВАТЕЛЬНО с разной скоростью
            foreach (var animal in enclosure.Animals)
            {
                AddToLog($"  🍽️ {animal.ReactToFood(e.FoodType)}");

                // Ждём пока съест (EatingSpeed секунд)
                await Task.Delay((int)(animal.EatingSpeed * 1000));

                AddToLog($"  ✅ {animal.Name} lõpetas söömise!");
            }

            AddToLog($"🏁 Kõik loomad voljeeri {e.EnclosureName} sõid!");
        }

        // ========== СУЩЕСТВУЮЩИЕ МЕТОДЫ (улучшенные) ==========

        private void SetupKeyboardShortcuts()
        {
            KeyBinding crazyBinding = new KeyBinding(
                new RelayCommand(CrazyActionShortcut),
                Key.D,
                ModifierKeys.Control
            );
            this.InputBindings.Add(crazyBinding);
        }

        private void CrazyActionShortcut(object parameter)
        {
            CrazyAction_Click(null, null);
        }

        private void AnimalsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                DetailsTextBlock.Text = selectedAnimal.Describe();
                AddToLog($"✅ Valitud: {selectedAnimal.Name}");
            }
            else
            {
                DetailsTextBlock.Text = "Vali loom vasakult...";
            }
        }

        private void MakeSound_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                string sound = selectedAnimal.MakeSound();
                AddToLog($"🔊 {selectedAnimal.Name} ütles: {sound}");
            }
            else
            {
                MessageBox.Show("Palun vali loom!", "Hoiatus", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Feed_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                string food = FoodTextBox.Text.Trim();

                if (string.IsNullOrEmpty(food))
                {
                    MessageBox.Show("Sisesta toit!", "Hoiatus", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                AddToLog($"🍖 {selectedAnimal.Name} sõi {food}. Nom nom nom!");
                FoodTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Palun vali loom!", "Hoiatus", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CrazyAction_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                if (selectedAnimal is ICrazyAction crazyAnimal)
                {
                    string result = crazyAnimal.ActCrazy();
                    AddToLog($"🎪 CRAZY! {result}");
                    DetailsTextBlock.Text = selectedAnimal.Describe();
                }
                else
                {
                    AddToLog($"❌ {selectedAnimal.Name} ei saa crazy action teha!");
                }
            }
            else
            {
                MessageBox.Show("Palun vali loom!", "Hoiatus", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Fly_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                if (selectedAnimal is IFlyable flyableAnimal)
                {
                    flyableAnimal.Fly();
                    string status = flyableAnimal.IsFlying ? "lendab nüüd" : "maandus";
                    AddToLog($"✈️ {selectedAnimal.Name} {status}!");
                    DetailsTextBlock.Text = selectedAnimal.Describe();
                }
                else
                {
                    AddToLog($"❌ {selectedAnimal.Name} ei oska lennata!");
                    MessageBox.Show($"{selectedAnimal.Name} ei saa lennata!",
                                  "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Palun vali loom!", "Hoiatus", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddAnimal_Click(object sender, RoutedEventArgs e)
        {
            AddAnimalDialog dialog = new AddAnimalDialog();
            if (dialog.ShowDialog() == true)
            {
                Animal newAnimal = dialog.CreatedAnimal;
                _animalRepository.Add(newAnimal);  // НОВОЕ: в Repository
                animals.Add(newAnimal);
                AddToLog($"➕ Lisatud uus loom: {newAnimal.Name} ({newAnimal.GetType().Name})");
                UpdateStatistics();  // НОВОЕ
            }
        }

        private void RemoveAnimal_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                var result = MessageBox.Show(
                    $"Kas oled kindel, et soovid eemaldada {selectedAnimal.Name}?",
                    "Kinnitus",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    _animalRepository.Remove(selectedAnimal);  // НОВОЕ
                    animals.Remove(selectedAnimal);
                    AddToLog($"❌ Eemaldatud: {selectedAnimal.Name}");
                    DetailsTextBlock.Text = "Vali loom vasakult...";
                    UpdateStatistics();  // НОВОЕ
                }
            }
            else
            {
                MessageBox.Show("Palun vali loom eemaldamiseks!", "Hoiatus",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ========== НОВЫЕ МЕТОДЫ ==========

        private void DropFood_Click(object sender, RoutedEventArgs e)
        {
            if (EnclosuresListBox.SelectedItem is Enclosure<Animal> selectedEnclosure)
            {
                string food = FoodTextBox.Text.Trim();
                if (string.IsNullOrEmpty(food))
                    food = "toit";

                selectedEnclosure.DropFood(food);
                FoodTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Vali voljeer!", "Hoiatus", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void StartTimer_Click(object sender, RoutedEventArgs e)
        {
            _eventManager.Start();
        }

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            _eventManager.Stop();
        }

        private void UpdateStatistics_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            statistics.Clear();

            // LINQ 1: GroupBy - группировка по типу
            var byType = _animalRepository.GetAll()
                .GroupBy(a => a.GetType().Name)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count(),
                    AvgAge = g.Average(a => a.Age)
                });

            statistics.Add("📊 STATISTIKA TÜÜBI JÄRGI (GroupBy):");
            foreach (var group in byType)
            {
                statistics.Add($"  {group.Type}: {group.Count} tk (keskmine vanus: {group.AvgAge:F1})");
            }

            // LINQ 2: OrderBy + Take - самые старые
            var oldest = _animalRepository.GetAll()
                .OrderByDescending(a => a.Age)
                .Take(3);

            statistics.Add("\n👴 VANIMAD LOOMAD (OrderBy + Take):");
            foreach (var animal in oldest)
            {
                statistics.Add($"  {animal.Name} - {animal.Age} aastat");
            }

            // LINQ 3: Where + Count - животные в вольерах
            var inEnclosures = _animalRepository.GetAll()
                .Where(a => !string.IsNullOrEmpty(a.EnclosureId))
                .GroupBy(a => a.EnclosureId);

            statistics.Add("\n🏠 LOOMAD VOLJEERIDES (Where + GroupBy):");
            foreach (var enc in inEnclosures)
            {
                statistics.Add($"  {enc.Key}: {enc.Count()} looma");
            }

            // LINQ 4: Aggregate функции
            var total = _animalRepository.GetAll().Count();
            var avgAge = _animalRepository.GetAll().Average(a => a.Age);
            var withCrazy = _animalRepository.GetAll().Count(a => a is ICrazyAction);
            var canFly = _animalRepository.GetAll().Count(a => a is IFlyable);

            statistics.Add($"\n📈 ÜLDSTATISTIKA (Count, Average):");
            statistics.Add($"  Kokku loomi: {total}");
            statistics.Add($"  Keskmine vanus: {avgAge:F1} aastat");
            statistics.Add($"  Crazy Action-ga: {withCrazy}");
            statistics.Add($"  Lendavad: {canFly}");
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            logEntries.Clear();
            AddToLog("🗑️ Logi tühjendatud.");
        }

        private void AddToLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            logEntries.Add($"[{timestamp}] {message}");

            if (LogListBox.Items.Count > 0)
            {
                LogListBox.ScrollIntoView(LogListBox.Items[LogListBox.Items.Count - 1]);
            }
        }
    }

    // Helper для keyboard shortcuts
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}