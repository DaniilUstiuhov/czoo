using System;
using System.Timers;
using System.Windows; // FIXED: Added for Dispatcher

namespace AnimalManager.Services
{
    public class NightEventArgs : EventArgs
    {
        public int DayNumber { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Менеджер событий зоопарка с таймером
    /// Каждые N секунд генерирует ночное событие
    /// FIXED: Now uses Dispatcher for thread-safe UI updates
    /// </summary>
    public class ZooEventManager
    {
        private Timer _nightTimer;
        private bool _isNight = false;
        private int _dayCount = 0;
        private readonly object _lockObject = new object(); // FIXED: Thread safety

        // События
        public event EventHandler<NightEventArgs> NightEvent;
        public event EventHandler<string> LogMessage;

        public bool IsRunning { get; private set; }

        public ZooEventManager(int intervalSeconds = 10)
        {
            _nightTimer = new Timer(intervalSeconds * 1000);
            _nightTimer.Elapsed += OnNightTimerElapsed;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                _nightTimer.Start();
                IsRunning = true;
                Log("🌙 Öösündmuste taimer käivitatud!");
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                _nightTimer.Stop();
                IsRunning = false;
                Log("☀️ Taimer peatatud.");
            }
        }

        // FIXED: Now thread-safe with Dispatcher
        private void OnNightTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Use Dispatcher to marshal to UI thread
            Application.Current?.Dispatcher.Invoke(() =>
            {
                lock (_lockObject) // Additional thread safety
                {
                    _isNight = !_isNight;

                    if (_isNight)
                    {
                        _dayCount++;
                        var eventArgs = new NightEventArgs
                        {
                            DayNumber = _dayCount,
                            Message = GenerateNightEvent()
                        };

                        OnNightEvent(eventArgs);
                    }
                    else
                    {
                        Log("☀️ Hommik on käes. Loomad ärkavad!");
                    }
                }
            });
        }

        private string GenerateNightEvent()
        {
            string[] events =
            {
                "🦉 Öökull alustas hiirte jahti",
                "🌙 Kõik loomad magavad tähistaeva all",
                "🐺 Hundid uluvad kuule",
                "🦝 Pesukarul õnnestus öine rööv köögist",
                "🦇 Nahkhiired läksid jahtima",
                "🌃 Öine valvur käib territooriumil ringi",
                "🦎 Roomajad soojendavad end infravalguslampide all"
            };

            return events[new Random().Next(events.Length)];
        }

        protected virtual void OnNightEvent(NightEventArgs e)
        {
            NightEvent?.Invoke(this, e);
            Log($"🌙 Päev {e.DayNumber}: {e.Message}");
        }

        private void Log(string message)
        {
            LogMessage?.Invoke(this, message);
        }

        public void ChangeInterval(int seconds)
        {
            _nightTimer.Interval = seconds * 1000;
            Log($"⏱️ Intervall muudetud: {seconds} sekundit");
        }

        // Cleanup
        public void Dispose()
        {
            _nightTimer?.Stop();
            _nightTimer?.Dispose();
        }
    }
}