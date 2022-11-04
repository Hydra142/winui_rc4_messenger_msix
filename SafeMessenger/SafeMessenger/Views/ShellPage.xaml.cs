using Microsoft.UI.Xaml.Controls;
using SafeMessenge.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SafeMessenge.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        public ShellPageViewModel ViewModel { get; set; }
        public ShellPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<ShellPageViewModel>();
            App.MainWindow.ExtendsContentIntoTitleBar = true;
            App.MainWindow.SetTitleBar(TitleBar);
            ViewModel.NavigationService.Frame = RootFrame;
            ViewModel.NavigationService.NavigateToLoginPage();
        }
    }
}
