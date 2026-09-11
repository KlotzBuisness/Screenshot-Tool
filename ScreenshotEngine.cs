using System;
using System.IO;

namespace GenericScreenshotTool
{
    public class ScreenshotEngine
    {
        #region constructor
        public readonly ScreenshotHelper _helper;
        // counter for better filename structure
        public int _counter = 1;

        public ScreenshotEngine(ScreenshotHelper helper)
        {
            _helper = helper;
        }
        #endregion

        #region public functions
        public void VisitTree(BaseViewModel node, Action<BaseViewModel> onNode)
        {
            if (node == null)
            {
                return;
            }

            // expands the current knot and forces UI to reload before action
            node.IsExpanded = true;
            _helper.DoRenderCycle();

            onNode?.Invoke(node);

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    VisitTree(child, onNode);
                }
            }

            // closes the knot when leaving (backtracking)
            node.IsExpanded = false;
            _helper.DoRenderCycle();
        }
       
        public void Shot(string name)
        {
            _helper.FileName = $"{_counter:D2}_{name}";
            _helper.TakeScreenshot();
            _counter++;
        }
        
        public void Navigate(Action navigateAction)
        {
            navigateAction?.Invoke();
            _helper.DoRenderCycle();
        }
        
        public void Scroll(Action scrollAction)
        {
            scrollAction?.Invoke();
            _helper.DoRenderCycle();
        }

        public void CopyFileToScreenshots(string sourceFilePath, string fileName)
        {
            if (File.Exists(sourceFilePath))
            {
                string destinationFilename = $"{_counter:D2}_{fileName}";
                string destinationPath = Path.Combine(_helper.FilePath, destinationFilename);

                File.Copy(sourceFilePath, destinationPath, true);
                _counter++;
            }
        }
        #endregion
    }
}
