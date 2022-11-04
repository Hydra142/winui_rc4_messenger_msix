using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SafeMessenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeMessenge.UserControls;
[ObservableObject]
public sealed partial class CreateChatDialog : UserControl
{
    [ObservableProperty]
    public List<User> _AvailableUsers = new();
    [ObservableProperty]
    public User? _SelectedUser = null;
    [ObservableProperty]
    public bool _IsListEmpty = true;
    [ObservableProperty]
    public bool _IsShowUsers = false;
    public CreateChatDialog()
    {
        InitializeComponent();
    }

    public async Task<User?> ShowAsync(List<User> availableUsers)
    {
        AvailableUsers = availableUsers;
        var isListEmpty = AvailableUsers.Count <= 0;

        SelectedUser = AvailableUsers.FirstOrDefault();
        SelectUserText.Visibility = isListEmpty ? Visibility.Collapsed : Visibility.Visible;
        UsersGridView.Visibility = isListEmpty ? Visibility.Collapsed : Visibility.Visible;
        CreateChatButton.Visibility = isListEmpty ? Visibility.Collapsed : Visibility.Visible;
        UnableSelectUserText.Visibility = !isListEmpty ? Visibility.Collapsed : Visibility.Visible;
        OkButton.Visibility = !isListEmpty ? Visibility.Collapsed : Visibility.Visible;
        
        await Dialog.ShowAsync();
        await Task.CompletedTask;
        return SelectedUser;
    }

    private void CloseDialog(object sender, RoutedEventArgs e)
    {
        Dialog.Hide();
    }

    private void Cancel(object sender, RoutedEventArgs e)
    {
        SelectedUser = null;
        Dialog.Hide();
    }
}
