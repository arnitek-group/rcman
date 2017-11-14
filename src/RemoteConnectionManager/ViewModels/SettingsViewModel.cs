using GalaSoft.MvvmLight;
using RemoteConnectionManager.Core.Services;
using RemoteConnectionManager.Models;
using RemoteConnectionManager.Services;
using System.Collections.Generic;
using System.Windows;

namespace RemoteConnectionManager.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly ITelemetryService _telemetryService;

        public SettingsViewModel(
            ISettingsService settingsService,
            ITelemetryService telemetryService)
        {
            _settingsService = settingsService;
            _telemetryService = telemetryService;
        }

        public double Width
        {
            get { return _settingsService.ApplicationSettings.Width; }
            set
            {
                if (_settingsService.ApplicationSettings.Width != value)
                {
                    _settingsService.ApplicationSettings.Width = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double Height
        {
            get { return _settingsService.ApplicationSettings.Height; }
            set
            {
                if (_settingsService.ApplicationSettings.Height != value)
                {
                    _settingsService.ApplicationSettings.Height = value;
                    RaisePropertyChanged();
                }
            }
        }

        public WindowState WindowState
        {
            get { return _settingsService.ApplicationSettings.WindowState; }
            set
            {
                if (_settingsService.ApplicationSettings.WindowState != value)
                {
                    _settingsService.ApplicationSettings.WindowState = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Theme Theme
        {
            get { return _settingsService.ApplicationSettings.Theme; }
            set
            {
                if (_settingsService.ApplicationSettings.Theme != value)
                {
                    _settingsService.ApplicationSettings.Theme = value;
                    RaisePropertyChanged();
                    SaveSettings();

                    _telemetryService.TrackEvent("Theme", new Dictionary<string, string>
                    {
                        {"Theme", _settingsService.ApplicationSettings.Theme.ToString() }
                    });
                }
            }
        }

        public void SaveSettings()
        {
            _settingsService.SaveSettings();
        }

        public string LayoutFile => _settingsService.ApplicationSettings.LayoutFile;
    }
}
