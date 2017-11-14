using System;
using GalaSoft.MvvmLight;
using RemoteConnectionManager.Models;
using RemoteConnectionManager.Services;
using System.Windows;

namespace RemoteConnectionManager.ViewModels
{
    public class SettingsViewModel: ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly ApplicationSettings _settings;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _settings = _settingsService.LoadSettings() ?? new ApplicationSettings
            {
                Width = double.NaN,
                Height = double.NaN,
                WindowState = WindowState.Maximized
            };
        }

        public double Width
        {
            get { return _settings.Width; }
            set
            {
                if (Math.Abs(_settings.Width - value) > 1)
                {
                    _settings.Width = value;
                    RaisePropertyChanged();
                }
            }
        }

        public double Height
        {
            get { return _settings.Height; }
            set
            {
                if (Math.Abs(_settings.Height - value) > 1)
                {
                    _settings.Height = value;
                    RaisePropertyChanged();
                }
            }
        }

        public WindowState WindowState
        {
            get { return _settings.WindowState; }
            set
            {
                if (_settings.WindowState != value)
                {
                    _settings.WindowState = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void SaveSettings()
        {
            _settingsService.SaveSettings(_settings);
        }

        public string LayoutFilePath => _settingsService.LayoutFilePath;
    }
}
