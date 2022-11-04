using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SafeMessenge.Models;
using SafeMessenge.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SafeMessenge.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page
{
    public LoginPageViewModel ViewModel { get; set; }
    public bool IsShowUSerSelection = false;
    public LoginPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<LoginPageViewModel>();
    }

    private void Login(object sender, RoutedEventArgs e)
    {
        if (ViewModel.CyrrentUser != null)
        {
            ViewModel.NavigationService.NavigateToMessengerPage();
        }
    }
}
