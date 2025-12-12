using System;
using System.Windows;
using System.Windows.Controls;

namespace AnimalManager
{
    public partial class AddAnimalDialog : Window
    {
        public Animal CreatedAnimal { get; private set; }
        private TextBox extraTextBox;

        public AddAnimalDialog()
        {
            InitializeComponent();
            AnimalTypeComboBox.SelectedIndex = 0;
        }

        private void AnimalTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Tühjendame lisainfo paneeli
            ExtraInfoPanel.Children.Clear();

            if (AnimalTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string tag = selectedItem.Tag.ToString();

                // Lisame spetsiifilised väljad vastavalt looma tüübile
                switch (tag)
                {
                    case "Cat":
                        AddExtraField("Lemmiktoit:");
                        break;
                    case "Dog":
                        AddExtraField("Tõug:");
                        break;
                    case "Bird":
                        AddExtraField("Värv:");
                        break;
                }
            }
        }

        private void AddExtraField(string label)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            extraTextBox = new TextBox
            {
                Height = 35,
                FontSize = 14,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(5)
            };

            ExtraInfoPanel.Children.Add(textBlock);
            ExtraInfoPanel.Children.Add(extraTextBox);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // IMPROVED: Enhanced validation

            // Validate name
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Palun sisesta nimi!", "Viga",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                NameTextBox.Focus();
                return;
            }

            // FIXED: Validate name length
            if (NameTextBox.Text.Trim().Length < 2)
            {
                MessageBox.Show("Nimi peab olema vähemalt 2 tähemärki pikk!", "Viga",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                NameTextBox.Focus();
                return;
            }

            if (NameTextBox.Text.Trim().Length > 50)
            {
                MessageBox.Show("Nimi ei tohi olla pikem kui 50 tähemärki!", "Viga",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                NameTextBox.Focus();
                return;
            }

            // Validate age
            if (!int.TryParse(AgeTextBox.Text, out int age) || age < 0 || age > 100)
            {
                MessageBox.Show("Palun sisesta korrektne vanus (0-100)!", "Viga",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                AgeTextBox.Focus();
                return;
            }

            // Loome looma vastavalt tüübile
            if (AnimalTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string name = NameTextBox.Text.Trim();
                string tag = selectedItem.Tag.ToString();
                string extraInfo = extraTextBox?.Text.Trim() ?? "";

                // FIXED: Validate extra info when required
                if (!string.IsNullOrEmpty(extraInfo))
                {
                    if (extraInfo.Length < 2)
                    {
                        MessageBox.Show("Lisakirje peab olema vähemalt 2 tähemärki pikk!", "Viga",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                        extraTextBox?.Focus();
                        return;
                    }

                    if (extraInfo.Length > 100)
                    {
                        MessageBox.Show("Lisakirje ei tohi olla pikem kui 100 tähemärki!", "Viga",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                        extraTextBox?.Focus();
                        return;
                    }
                }

                switch (tag)
                {
                    case "Cat":
                        if (string.IsNullOrWhiteSpace(extraInfo)) extraInfo = "kala";
                        CreatedAnimal = new Cat(name, age, extraInfo);
                        break;
                    case "Dog":
                        if (string.IsNullOrWhiteSpace(extraInfo)) extraInfo = "Segaverelne";
                        CreatedAnimal = new Dog(name, age, extraInfo);
                        break;
                    case "Bird":
                        if (string.IsNullOrWhiteSpace(extraInfo)) extraInfo = "sinine";
                        CreatedAnimal = new Bird(name, age, extraInfo);
                        break;
                    case "Raccoon":
                        CreatedAnimal = new Raccoon(name, age);
                        break;
                    case "Monkey":
                        CreatedAnimal = new Monkey(name, age);
                        break;
                }

                DialogResult = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}