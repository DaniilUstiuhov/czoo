using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace AnimalManager
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Animal> animals;
        private ObservableCollection<string> logEntries;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            SetupKeyboardShortcuts();
        }

        private void InitializeData()
        {
            // Loome loomade kollektsiooni
            animals = new ObservableCollection<Animal>
            {
                new Cat("Muri", 3, "juustu"),
                new Dog("Rex", 5, "Saksa lambakoer"),
                new Bird("Piip", 2, "kollane"),
                new Raccoon("Riku", 4),
                new Monkey("Mango", 6)
            };

            // Loome logi kollektsiooni
            logEntries = new ObservableCollection<string>();

            // Seome kollektsioonid UI-ga
            AnimalsListBox.ItemsSource = animals;
            LogListBox.ItemsSource = logEntries;

            // Lisa tervitussõnum logisse
            AddToLog("🎉 Tere tulemast loomade haldamise süsteemi!");
            AddToLog($"📊 Laaditud {animals.Count} looma.");
        }

        private void SetupKeyboardShortcuts()
        {
            // Ctrl+D = Crazy Action
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

                    // Uuendame detaile
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

                    // Uuendame detaile
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
            // Avame dialoogi uue looma lisamiseks
            AddAnimalDialog dialog = new AddAnimalDialog();
            if (dialog.ShowDialog() == true)
            {
                Animal newAnimal = dialog.CreatedAnimal;
                animals.Add(newAnimal);
                AddToLog($"➕ Lisatud uus loom: {newAnimal.Name} ({newAnimal.GetType().Name})");
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
                    animals.Remove(selectedAnimal);
                    AddToLog($"❌ Eemaldatud: {selectedAnimal.Name}");
                    DetailsTextBlock.Text = "Vali loom vasakult...";
                }
            }
            else
            {
                MessageBox.Show("Palun vali loom eemaldamiseks!", "Hoiatus",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

            // Kerime logi lõppu
            if (LogListBox.Items.Count > 0)
            {
                LogListBox.ScrollIntoView(LogListBox.Items[LogListBox.Items.Count - 1]);
            }
        }
    }

    // Helper klass klaviatuuri kiirklahvideks
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