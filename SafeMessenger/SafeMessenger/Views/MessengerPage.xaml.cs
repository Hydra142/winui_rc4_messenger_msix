using Microsoft.UI;
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SafeMessenge.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MessengerPage : Page
    {
        public MessengerViewModel ViewModel { get; set; }
        public MessengerPage()
        {
            ViewModel = App.GetService<MessengerViewModel>();
            this.InitializeComponent();
        }

        private void BackToLogin(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentChatData = null;
            ViewModel.StartUpdateTimer(true);
            ViewModel.NavigationService.NavigateToLoginPage();
        }

        private async void OpenCreateChatDialog(object sender, RoutedEventArgs e)
        {
            LoadingNewChatIcon.Visibility = Visibility.Visible;
            CreateChatIcon.Visibility = Visibility.Collapsed;
            CreateChatBtn.IsEnabled = false;
            var SelectedUser = await CreateChatContentDialog.ShowAsync(ViewModel.getAvailableUsersToCreateChat());
            if (SelectedUser != null)
            {
                ViewModel.CreatingChatUserId = SelectedUser.Id;
                await ViewModel.CreateChat(SelectedUser);
                Chats.SelectedIndex = 0;
            }
            LoadingNewChatIcon.Visibility = Visibility.Collapsed;
            CreateChatIcon.Visibility = Visibility.Visible;
            CreateChatBtn.IsEnabled = true;
        }

        private async void SentMessage(object sender, RoutedEventArgs e)
        {
            if (ViewModel.InputMessageText != "" && ViewModel.IsSentMessageEnabled)
            {
                ShowLoading(true);
                await ViewModel.SentMessage();
                ShowLoading(false);
            }
        }

        private async void SentMessageByEnter(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && ViewModel.IsSentMessageEnabled)
            {
                TextBox txtBox = (TextBox)e.OriginalSource;
                if (txtBox != null)
                {
                    ViewModel.InputMessageText = txtBox.Text;
                    ShowLoading(true);
                    await ViewModel.SentMessage();
                    ShowLoading(false);
                }
            }
        }

        private void ShowLoading(bool isLoading)
        {
            SentMessageBtn.Visibility = isLoading ? Visibility.Collapsed : Visibility.Visible;
            LoadingMessageIcon.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ShowHideEncodingKeySettingsGrid(object sender, RoutedEventArgs e)
        {
            var isVisible = EncodingKeySettingsGrid.Visibility == Visibility.Visible;
            EncodingKeySettingsGrid.Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void EncriptingTextKeyChanged(object sender, TextChangedEventArgs e)
        {
            if(ViewModel.CurrentChatData != null)
            {
                ViewModel.CurrentChatData.DecodeMessages();
            }
            
        }
    }
}
