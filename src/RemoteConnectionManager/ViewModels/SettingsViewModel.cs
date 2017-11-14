using System.Windows;
using GalaSoft.MvvmLight;
using RemoteConnectionManager.Models;
using RemoteConnectionManager.Services;

namespace RemoteConnectionManager.ViewModels
{
    public class SettingsViewModel: ViewModelBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            Settings = _settingsService.LoadSettings() ?? new ApplicationSettings
            {
                Width = double.NaN,
                Height = double.NaN,
                WindowState = WindowState.Maximized
            };
        }

        public ApplicationSettings Settings { get; }

        public void SaveSettings()
        {
            _settingsService.SaveSettings(Settings);
        }
    }
}
