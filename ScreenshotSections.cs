using System;
using System.Collections.Generic;
using System.Linq;

namespace ScreenshotTool
{
    public class ScreenshotSections
    {
        #region constructor
        private readonly ScreenshotEngine _engine;
        private readonly MainWindowViewModel _mainViewModel;

        // Uses Dependency Injection (DI) for clean separation of engine and viewmodel
        public ScreenshotSections(ScreenshotEngine engine, MainWindowViewModel mainViewModel)
        {
            _engine = engine;
            _mainViewModel = mainViewModel;
        }
        #endregion

        #region public functions
        public void ScreenshotHome()
        {
            _engine._helper.SetOrCreateFolder(ScreenshotSection.Home);
            _engine.Navigate(_mainViewModel.ShowHome);
            _engine.Shot("01_Homescreen");
        }

        public void ScreenshotSettings()
        {
            _engine._helper.SetOrCreateFolder(ScreenshotSection.Settings);

            // Uses safe intialization (prevents system crashes, if there's no data)
            var settings = SettingsViewModel.Children?.ToList() ?? new List<BaseViewModel>();

            foreach (var settingsChild in settings)
            {
                _engine.Navigate(ShowSettingsWindow);
                _mainViewModel.SettingsViewModel.SelectedItem = settingsChild;
                
                // forces the automatic scroll of the UI to the selected element before screenhot
                _engine.Scroll(DataGridScrollHelper.RequestScrollToSelectedItem);
                _engine.Shot(settingsChild.DisplayName);
            }
        }

        public void ScreenshotDashboard()
        {
            _engine._helper.SetOrCreateFolder(ScreenshotSection.Dashboard);

            var dashboards = DashboardViewModel.Children?.ToList() ?? new List<BaseViewModel>();

            foreach (var dashboardChild in dashboards)
            {
                _engine.Navigate(ShowDashboardWindow);
                _mainViewModel.DashboardViewModel.SelectedItem = dashboardChild;
                
                _engine.Scroll(DataGridScrollHelper.RequestScrollToSelectedItem);
                _engine.Shot(dashboardChild.DisplayName);
            }
        }
      
        public void ScreenshotAnalytics()
        {
            _engine._helper.SetOrCreateFolder(ScreenshotSection.Analytics);

            var analytics = AnalyticsViewModel.Children?.ToList() ?? new List<BaseViewModel>();

            foreach (var analyticsChild in analytics)
            {
                _engine.Navigate(ShowAnalyticsWindow);
                _mainViewModel.AnalyticsViewModel.SelectedItem = analyticsChild;
                
                _engine.Scroll(DataGridScrollHelper.RequestScrollToSelectedItem);
                _engine.Shot(analyticsChild.DisplayName);
            }
        }

        public void RunAll()
        {
            ScreenshotHome();
            ScreenshotSettings();
            ScreenshotDashboard();
            ScreenshotAnalytics();
        }
    }
}
