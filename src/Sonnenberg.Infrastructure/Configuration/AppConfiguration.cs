using Sonnenberg.Core.Models;

namespace Sonnenberg.Infrastructure.Configuration
{
    /// <summary>
    /// Thread-safe singleton that holds application configuration and state
    /// </summary>
    public sealed class AppConfiguration
    {
        private static readonly object _lock = new object();
        private static AppConfiguration _instance;

        private ItemContext _currentContext;
        private bool _hasMenuWatcherSubscribed;

        private AppConfiguration()
        {
        }

        public static AppConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new AppConfiguration();
                        }
                    }
                }
                return _instance;
            }
        }

        public ItemContext CurrentContext
        {
            get
            {
                lock (_lock)
                {
                    return _currentContext;
                }
            }
            set
            {
                lock (_lock)
                {
                    _currentContext = value;
                }
            }
        }

        public bool HasMenuWatcherSubscribed
        {
            get
            {
                lock (_lock)
                {
                    return _hasMenuWatcherSubscribed;
                }
            }
            set
            {
                lock (_lock)
                {
                    _hasMenuWatcherSubscribed = value;
                }
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                _currentContext = null;
                _hasMenuWatcherSubscribed = false;
            }
        }
    }
}
