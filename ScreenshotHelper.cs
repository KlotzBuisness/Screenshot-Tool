using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GenericScreenshotTool
{
    #region enums
    public enum ScreenshotSection
    {
        Home = 0,
        Settings = 1,
        Dashboard = 2,
        Analytics = 3
    }
    #endregion

    public class ScreenshotHelper
    {
        #region variables
        public string FilePath { get; set; }
        public string FileName { get; set; } = "unknown_capture";
        #endregion

        #region private functions
        private void ClearFolderContents(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return;

            foreach (string file in Directory.GetFiles(folderPath))
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine($"Error deleting file {file}: {exception.Message}");
                }
            }

            foreach (string directory in Directory.GetDirectories(folderPath))
            {
                try
                {
                    Directory.Delete(directory, true);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine($"Error deleting folder {directory}: {exception.Message}");
                }
            }
        }

        private string GetSectionFolderName(ScreenshotSection section)
        {
            // examples are used for demo purpose
            return section switch
            {
                ScreenshotSection.Home => "01_Home",
                ScreenshotSection.Settings => "02_Settings",
                ScreenshotSection.Dashboard => "03_Dashboard",
                ScreenshotSection.Analytics => "04_Analytics",
                _ => "00_General"
            };
        }
        #endregion

        #region public functions
        public void SetOrCreateFolder(ScreenshotSection section = ScreenshotSection.Home)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string dataPath = Path.Combine(basePath, "ExportData");
            string screenshotBasePath = Path.Combine(dataPath, "AppScreenshots");
            
            // gets the Systemlanguage (f.e. "de" or "en") for localized folders
            string language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            string languageFolderPath = Path.Combine(screenshotBasePath, language);

            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            if (!Directory.Exists(screenshotBasePath)) Directory.CreateDirectory(screenshotBasePath);
            if (!Directory.Exists(languageFolderPath)) Directory.CreateDirectory(languageFolderPath);

            //selects the subfolder based on the section
            string sectionFolderName = GetSectionFolderName(section);
            string sectionFolderPath = Path.Combine(languageFolderPath, sectionFolderName);

            if (!Directory.Exists(sectionFolderPath))
            {
                Directory.CreateDirectory(sectionFolderPath);
            }
            else
            {
                ClearFolderContents(sectionFolderPath);
            }

            FilePath = sectionFolderPath;
        }

        public void TakeScreenshot()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;

            if (mainWindow == null)
            {
                Debug.WriteLine("No active MainWindow found.");
                return;
            }

            // forces the layout update of the wpf-window before the screenshot
            mainWindow.UpdateLayout();
            DoRenderCycle();

            int width = (int)mainWindow.ActualWidth;
            int height = (int)mainWindow.ActualHeight;

            if (width == 0 || height == 0) return;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                        
            var screenshotObject = LogicalTreeHelper.FindLogicalNode(mainWindow, "MainRenderArea"); // fictive generic knot name for demo purpose
            
            Visual visualToRender = (screenshotObject is Visual) ? (Visual)screenshotObject : mainWindow; // if specific element is not found, takes a picture fo the whole window

            try
            {
                renderTargetBitmap.Render(visualToRender);

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                string fullPath = Path.Combine(FilePath, $"{FileName}.png");

                using (var fileSave = new FileStream(fullPath, FileMode.Create))
                {
                    encoder.Save(fileSave);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save screenshot: {ex.Message}");
            }
        }

        public void DoRenderCycle()
        {
            // Empty Invokes on the UI-Thread forces WPF to finish every layout-
            // and rendering-process (important for fast page changes
            System.Windows.Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
            System.Windows.Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Render);
        }
        #endregion
    }
}
