using System;
using System.Windows;

namespace AnimalManager
{
    public static class Program
    {
        [STAThread]  // Обязательно для WPF UI-потока
        public static void Main()
        {
            // Создаём приложение
            var app = new App();

            // Загружаем компоненты из App.xaml
            app.InitializeComponent();

            // Запускаем цикл обработки сообщений (UI)
            app.Run();
        }
    }
}
