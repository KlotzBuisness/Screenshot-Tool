public void screenshotFunction()
{
    // launces every Helper Section
    var helper = new ScreenshotHelper();
    var engine = new ScreenshotEngine(helper);
    var sections = new ScreenshotSections(engine, MainViewModel);
    
    helper.SetOrCreateFolder(); // creates folder if not existing

    try
    {
        System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
        MainViewModel.ScreenshotVisibility = System.Windows.Visibility.Visible; // creates a UI Blocker, to prevent function from breaking
        sections.RunAll(); // runs the whole function
    }
    catch (Exception exception)
    {
        Log.Error(exception.ToString());
    }
    finally
    {
        MainVM.MainSelection.ShowFirstPage();
        MainVM.ScreenshotVisibility = System.Windows.Visibility.Collapsed;
        System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        Log.Info("Screenshot done");
        MessageBox.Show("Screenshot is finished);
    }
}
