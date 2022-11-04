using Microsoft.UI.Xaml.Controls;
using SafeMessenge.Views;

namespace SafeMessenge.Services;

public class NavigationService
{
    private Frame? _frame;
    public Frame? Frame
    {
        get
        {
            if (_frame == null)
            {
                _frame = App.MainWindow.Content as Frame;
            }

            return _frame;
        }

        set
        {
            _frame = value;
        }
    }
    
    public void NavigateToLoginPage()
    {
        if (Frame != null)
        {
            Frame.Navigate(typeof(LoginPage));
        }
    }

    public void NavigateToMessengerPage()
    {
        if (Frame != null)
        {
            Frame.Navigate(typeof(MessengerPage));
        }
    }
}
