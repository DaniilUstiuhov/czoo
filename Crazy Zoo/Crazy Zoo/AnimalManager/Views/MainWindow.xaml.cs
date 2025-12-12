using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AnimalManager.Interfaces;
using AnimalManager.Repositories;
using AnimalManager.Services;
using AnimalManager.Resources;
using Microsoft.Win32;

namespace AnimalManager
{
    public partial class MainWindow : Window
    {
        // Dependency Injection
        private readonly ILogger _logger;
        private readonly IAnimalRepository _dbRepository;
        private readonly ZooEventManager _eventManager;

        // In-memory repository for UI
        private readonly IRepository<Animal> _animalRepository;

        private ObservableCollection<Animal> animals;
        private ObservableCollection<string> logEntries;
        private ObservableCollection<string> statistics;
        private ObservableCollection<Enclosure<Animal>> enclosures;

        // Localization
        private readonly Localization _loc = Localization.Instance;

        public MainWindow(ILogger logger, IAnimalRepository dbRepository, ZooEventManager eventManager)
        {
            InitializeComponent();

            // Store injected dependencies
            _logger = logger;
            _dbRepository = dbRepository;
            _eventManager = eventManager;

            // Initialize in-memory repository
            _animalRepository = new InMemoryRepository<Animal>();

            // Initialize collections
            animals = new ObservableCollection<Animal>();
            logEntries = new ObservableCollection<string>();
            statistics = new ObservableCollection<string>();
            enclosures = new ObservableCollection<Enclosure<Animal>>();

            // Setup event manager
            _eventManager.LogMessage += (s, msg) => AddToLog(msg);
            _eventManager.NightEvent += (s, e) => AddToLog($"🌙 {e.Message}");

            // Bind to UI
            AnimalsListBox.ItemsSource = animals;
            LogListBox.ItemsSource = logEntries;
            StatisticsListBox.ItemsSource = statistics;
            EnclosuresListBox.ItemsSource = enclosures;

            // Setup localization binding
            DataContext = _loc;
            this.Title = _loc["WindowTitle"];

            SetupKeyboardShortcuts();
            InitializeData();
        }

        private void InitializeData()
        {
            // Create enclosures
            var enclosure1 = new Enclosure<Animal>("Voljeer A", 5);
            var enclosure2 = new Enclosure<Animal>("Voljeer B", 5);
            var enclosure3 = new Enclosure<Animal>("Voljeer C", 3);

            // Subscribe to enclosure events
            enclosure1.AnimalJoinedInSameEnclosure += OnAnimalJoinedEnclosure;
            enclosure1.FoodDropped += OnFoodDroppedInEnclosure;
            enclosure2.AnimalJoinedInSameEnclosure += OnAnimalJoinedEnclosure;
            enclosure2.FoodDropped += OnFoodDroppedInEnclosure;
            enclosure3.AnimalJoinedInSameEnclosure += OnAnimalJoinedEnclosure;
            enclosure3.FoodDropped += OnFoodDroppedInEnclosure;

            enclosures.Add(enclosure1);
            enclosures.Add(enclosure2);
            enclosures.Add(enclosure3);

            // Create animals
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

            // Add to repository and UI
            foreach (var animal in animalsList)
            {
                _animalRepository.Add(animal);
                animals.Add(animal);
            }

            // Distribute to enclosures
            enclosure1.AddAnimal(animalsList[0]);
            enclosure1.AddAnimal(animalsList[1]);
            enclosure2.AddAnimal(animalsList[2]);
            enclosure2.AddAnimal(animalsList[3]);
            enclosure3.AddAnimal(animalsList[4]);

            AddToLog(_loc["Welcome"]);
            AddToLog(string.Format(_loc["AnimalsLoaded"], animals.Count, enclosures.Count));

            UpdateStatistics();
        }

        // ========== EVENT HANDLERS ==========

        private void OnAnimalJoinedEnclosure(object sender, AnimalEventArgs e)
        {
            var enclosure = sender as Enclosure<Animal>;
            AddToLog(string.Format(_loc["AnimalJoined"], e.Animal.Name, e.EnclosureName));

            // All neighbors react
            foreach (var animal in enclosure.Animals.Where(a => a != e.Animal))
            {
                AddToLog($"  💬 {animal.ReactToNewNeighbor(e.Animal)}");
            }
        }

        private async void OnFoodDroppedInEnclosure(object sender, FoodEventArgs e)
        {
            var enclosure = sender as Enclosure<Animal>;
            AddToLog(string.Format(_loc["FoodDropped"], e.FoodType, e.EnclosureName));

            // Animals eat SEQUENTIALLY with different speeds
            foreach (var animal in enclosure.Animals)
            {
                AddToLog(string.Format(_loc["EatingStarted"], animal.ReactToFood(e.FoodType)));

                // Wait while eating (EatingSpeed seconds)
                await Task.Delay((int)(animal.EatingSpeed * 1000));

                AddToLog(string.Format(_loc["EatingFinished"], animal.Name));
            }

            AddToLog(string.Format(_loc["AllEaten"], e.EnclosureName));
        }

        // ========== EXISTING METHODS ==========

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
                AddToLog(string.Format(_loc["AnimalSelected"], selectedAnimal.Name));
            }
            else
            {
                DetailsTextBlock.Text = _loc["SelectAnimal"];
            }
        }

        private void MakeSound_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                string sound = selectedAnimal.MakeSound();
                AddToLog(string.Format(_loc["SoundMade"], selectedAnimal.Name, sound));
            }
            else
            {
                MessageBox.Show(_loc["PleaseSelectAnimal"], _loc["Warning"],
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Feed_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                string food = FoodTextBox.Text.Trim();

                if (string.IsNullOrEmpty(food))
                {
                    MessageBox.Show(_loc["EnterFood"], _loc["Warning"],
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                AddToLog(string.Format(_loc["AnimalFed"], selectedAnimal.Name, food));
                FoodTextBox.Clear();
            }
            else
            {
                MessageBox.Show(_loc["PleaseSelectAnimal"], _loc["Warning"],
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show(_loc["PleaseSelectAnimal"], _loc["Warning"],
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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
                }
            }
            else
            {
                MessageBox.Show(_loc["PleaseSelectAnimal"], _loc["Warning"],
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddAnimal_Click(object sender, RoutedEventArgs e)
        {
            AddAnimalDialog dialog = new AddAnimalDialog();
            if (dialog.ShowDialog() == true)
            {
                Animal newAnimal = dialog.CreatedAnimal;
                _animalRepository.Add(newAnimal);
                animals.Add(newAnimal);
                AddToLog(string.Format(_loc["AnimalAdded"], newAnimal.Name, newAnimal.GetType().Name));
                UpdateStatistics();
            }
        }

        private void RemoveAnimal_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                var result = MessageBox.Show(
                    _loc["ConfirmRemove"],
                    _loc["Warning"],
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    // FIXED: Remove from enclosure first
                    var enclosure = enclosures.FirstOrDefault(enc => enc.Animals.Contains(selectedAnimal));
                    if (enclosure != null)
                    {
                        enclosure.RemoveAnimal(selectedAnimal);
                        AddToLog($"🏠 {selectedAnimal.Name} eemaldatud voljeerist {enclosure.Name}");
                    }

                    // Then remove from repositories
                    _animalRepository.Remove(selectedAnimal);
                    animals.Remove(selectedAnimal);

                    AddToLog(string.Format(_loc["AnimalRemoved"], selectedAnimal.Name));
                    DetailsTextBlock.Text = _loc["SelectAnimal"];
                    UpdateStatistics();
                }
            }
            else
            {
                MessageBox.Show(_loc["PleaseSelectAnimal"], _loc["Warning"],
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // ========== NEW METHODS ==========

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
                MessageBox.Show(_loc["SelectEnclosure"], _loc["Warning"],
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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

            // LINQ 1: GroupBy - grouping by type
            var byType = _animalRepository.GetAll()
                .GroupBy(a => a.GetType().Name)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count(),
                    AvgAge = g.Average(a => a.Age)
                });

            statistics.Add(_loc["StatsByType"]);
            foreach (var group in byType)
            {
                statistics.Add($"  {group.Type}: {group.Count} tk (keskmine vanus: {group.AvgAge:F1})");
            }

            // LINQ 2: OrderBy + Take - oldest animals
            var oldest = _animalRepository.GetAll()
                .OrderByDescending(a => a.Age)
                .Take(3);

            statistics.Add("\n" + _loc["OldestAnimals"]);
            foreach (var animal in oldest)
            {
                statistics.Add($"  {animal.Name} - {string.Format(_loc["YearsOld"], animal.Age)}");
            }

            // LINQ 3: Where + Count - animals in enclosures
            var inEnclosures = _animalRepository.GetAll()
                .Where(a => !string.IsNullOrEmpty(a.EnclosureId))
                .GroupBy(a => a.EnclosureId);

            statistics.Add("\n" + _loc["AnimalsInEnclosures"]);
            foreach (var enc in inEnclosures)
            {
                statistics.Add($"  {enc.Key}: {enc.Count()} looma");
            }

            // LINQ 4: Aggregate functions
            var total = _animalRepository.GetAll().Count();
            var avgAge = _animalRepository.GetAll().Average(a => a.Age);
            var withCrazy = _animalRepository.GetAll().Count(a => a is ICrazyAction);
            var canFly = _animalRepository.GetAll().Count(a => a is IFlyable);

            statistics.Add("\n" + _loc["GeneralStats"]);
            statistics.Add(string.Format(_loc["TotalAnimals"], total));
            statistics.Add(string.Format(_loc["AverageAge"], avgAge));
            statistics.Add(string.Format(_loc["WithCrazy"], withCrazy));
            statistics.Add(string.Format(_loc["CanFly"], canFly));
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            logEntries.Clear();
            _logger.Clear();
            AddToLog(_loc["LogCleared"]);
        }

        private void AddToLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string fullMessage = $"[{timestamp}] {message}";
            logEntries.Add(fullMessage);
            _logger.Log(message);

            if (LogListBox.Items.Count > 0)
            {
                LogListBox.ScrollIntoView(LogListBox.Items[LogListBox.Items.Count - 1]);
            }
        }

        // ========== LOGGER OPERATIONS ==========

        private void SaveLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml|JSON Files (*.json)|*.json",
                    DefaultExt = _logger is XmlLogger ? "xml" : "json",
                    FileName = $"AnimalLog_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (dialog.ShowDialog() == true)
                {
                    _logger.SaveToFile(dialog.FileName);
                    AddToLog(string.Format(_loc["LogsSaved"], dialog.FileName));
                    MessageBox.Show(_loc["Success"], _loc["Success"],
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{_loc["Error"]}: {ex.Message}", _loc["Error"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml|JSON Files (*.json)|*.json",
                    DefaultExt = _logger is XmlLogger ? "xml" : "json"
                };

                if (dialog.ShowDialog() == true)
                {
                    _logger.LoadFromFile(dialog.FileName);

                    logEntries.Clear();
                    foreach (var log in _logger.GetLogs())
                    {
                        logEntries.Add(log);
                    }

                    AddToLog(string.Format(_loc["LogsLoaded"], dialog.FileName));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{_loc["Error"]}: {ex.Message}", _loc["Error"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ========== DATABASE OPERATIONS ==========

        private void SaveToDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Clear existing data
                _dbRepository.ClearAll();

                // Save enclosures
                foreach (var enclosure in enclosures)
                {
                    _dbRepository.AddEnclosure(enclosure.Name, enclosure.Capacity);
                }

                // Save animals
                foreach (var animal in animals)
                {
                    _dbRepository.AddAnimal(animal);
                }

                AddToLog(_loc["DataSaved"]);
                MessageBox.Show(_loc["Success"], _loc["Success"],
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{_loc["Error"]}: {ex.Message}", _loc["Error"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFromDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Clear current data
                animals.Clear();

                // FIXED: Create a list copy before iteration
                var animalsToRemove = _animalRepository.GetAll().ToList();
                foreach (var animal in animalsToRemove)
                {
                    _animalRepository.Remove(animal);
                }

                // Load animals from database
                var dbAnimals = _dbRepository.GetAllAnimals();
                foreach (var animal in dbAnimals)
                {
                    _animalRepository.Add(animal);
                    animals.Add(animal);
                }

                // Update enclosures with loaded animals
                foreach (var enclosure in enclosures)
                {
                    var animalsInEnclosure = animals.Where(a => a.EnclosureId == enclosure.Name).ToList();
                    foreach (var animal in animalsInEnclosure)
                    {
                        enclosure.AddAnimal(animal);
                    }
                }

                AddToLog(_loc["DataLoaded"]);
                UpdateStatistics();
                MessageBox.Show(_loc["Success"], _loc["Success"],
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{_loc["Error"]}: {ex.Message}", _loc["Error"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ========== LOCALIZATION ==========

        private void SwitchLanguage_Click(object sender, RoutedEventArgs e)
        {
            _loc.SwitchLanguage();

            // Update window title
            this.Title = _loc["WindowTitle"];

            // Update details text if nothing is selected
            if (AnimalsListBox.SelectedItem == null)
            {
                DetailsTextBlock.Text = _loc["SelectAnimal"];
            }

            AddToLog($"🌐 Language switched to: {_loc.CurrentLanguage}");
        }
    }

    // Helper for keyboard shortcuts
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