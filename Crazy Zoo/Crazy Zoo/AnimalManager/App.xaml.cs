using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AnimalManager.Interfaces;
using AnimalManager.Services;
using AnimalManager.Data;

namespace AnimalManager
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            // Configure Dependency Injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register Logger (switch between XmlLogger and JsonLogger)
            // Uncomment the one you want to use:
            
            services.AddSingleton<ILogger, XmlLogger>();  // XML implementation
            // services.AddSingleton<ILogger, JsonLogger>();  // JSON implementation

            // Register Animal Repository
            services.AddSingleton<IAnimalRepository, LocalDbAnimalRepository>();

            // Register ZooEventManager
            services.AddSingleton<ZooEventManager>(sp => new ZooEventManager(10));

            // Register MainWindow
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
