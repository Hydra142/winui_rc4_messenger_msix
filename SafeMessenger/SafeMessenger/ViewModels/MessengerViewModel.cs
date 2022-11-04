using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using SafeMessenge.Helpers;
using SafeMessenge.Models;
using SafeMessenge.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace SafeMessenge.ViewModels
{
    public class MessengerViewModel : ObservableRecipient
    {
        public NavigationService NavigationService { get; set; }
        public AppDataService AppDataService { get; set; }
        public ObservableCollection<Chat> UserChats = new();
        public System.Windows.Threading.DispatcherTimer UpdateTimer = new();
        private User _CurrentUser = new();
        private List<User> _Users = new();
        private Chat? _CurrentChatData;
        private string _InputMessageText = "";
        private string _HelpMessageText = "";
        private bool _IsSentMessageEnabled = true;
        private bool _IsMonitoringRunning = false;
        private long _CreatingChatUserId;
        public long CreatingChatUserId
        {
            get => _CreatingChatUserId;
            set => SetProperty(ref _CreatingChatUserId, value);
        }
        public User CurrentUser 
        {
            get => _CurrentUser;
            set => SetProperty(ref _CurrentUser, value);
        }
        public List<User> Users
        {
            get => _Users;
            set => SetProperty(ref _Users, value);
        }
        public Chat? CurrentChatData
        {
            get => _CurrentChatData;
            set
            {
                if (value != null)
                {
                    value.ChatMessages.CollectionChanged += (sender, e) => {
                        HelpMessageText = e != null && e.NewItems != null && e.NewItems.Count > 0 ? "" : "Історія чату пуста";
                    };
                    HelpMessageText = value.ChatMessages.Any() ? "" : "Історія чату пуста";
                    value.IsNewMessages = false;
                    value.NewMessagesCount = 0;
                }
                SetProperty(ref _CurrentChatData, value);
            }
        }
        public string InputMessageText
        {
            get => _InputMessageText;
            set => SetProperty(ref _InputMessageText, value);
        }
        public string HelpMessageText
        {
            get => _HelpMessageText;
            set => SetProperty(ref _HelpMessageText, value);
        }

        public bool IsSentMessageEnabled
        {
            get => _IsSentMessageEnabled;
            set => SetProperty(ref _IsSentMessageEnabled, value);
        }

        public MessengerViewModel(NavigationService navigationService, AppDataService appDataService)
        {
            NavigationService = navigationService;
            AppDataService = appDataService;
            var currentUser = AppDataService.CurrentUser;
            if (currentUser != null)
            {
                CurrentUser = currentUser;
            } else
            {
                NavigationService.NavigateToLoginPage();
            }
            UserChats.CollectionChanged += (sender, e) => {
                HelpMessageText = e != null && e.NewItems != null && e.NewItems.Count > 0 ? (CurrentChatData == null ? "Виберіть чат" : "")  : "Список чатів пустий, додайте чат щоб почати спілкування";
            };
            HelpMessageText = UserChats.Any() ? "Виберіть чат2" : "Список чатів пустий, додайте чат щоб почати спілкування";
            _Users = AppDataService.Users;
            SetUserChats();
            StartUpdateTimer();
        }

        public List<User> getAvailableUsersToCreateChat()
        {
            var users = GetOtherUsers();
            var availableUsers = users.Where(x => !UserChats.Select(x => x.PartnerId).Contains(x.Id)).ToList();
            System.Diagnostics.Debug.WriteLine($"available = {string.Join(",", availableUsers.Select(x =>x.Id).ToArray())}");
            return availableUsers;
        }

        public List<User> GetOtherUsers()
        {
            var users = Users.Where(x => x.Id != CurrentUser.Id).ToList();
            return users;
        }

        public async Task CreateChat(User user)
        {
            var createdChat = await AppDataService.ApiService.SendPostRequest<Chat>("chats", new { users_id_array = new long[] { CurrentUser.Id, user.Id } });
            if (createdChat!= null)
            {
                createdChat.Init(Users, CurrentUser.Id);
                UserChats.Insert(0, createdChat);
                CurrentChatData = createdChat;
            }
            await Task.CompletedTask;
        }

        public List<Chat> SetUserChats()
        {
            var userChats = AppDataService.SetUserChatsFromApi(CurrentUser.Id);
            userChats.ForEach(chat => chat.Init(Users, CurrentUser.Id));
            userChats.ForEach(chat => UserChats.Add(chat));
            return userChats;
        }

        public async Task SentMessage()
        {
            if (CurrentChatData != null && CurrentUser != null && InputMessageText != "" && IsSentMessageEnabled)
            {
                IsSentMessageEnabled = false;
                var chiper = new RC4Helper(CurrentChatData.EncriptingTextKey);
                var chatId = CurrentChatData.Id;
                var encryptedMessage = chiper.EncryptStringMessage(InputMessageText);
                var messageType = "text";
                var userSentId = CurrentUser.Id;
                var message = new
                {
                    chat_id = chatId,
                    encrypted_message = encryptedMessage,
                    message_type = messageType,
                    user_sent_id = userSentId,
                    sent_date = DateTime.Now,
                };
                InputMessageText = "";
                var createdMessage = await AppDataService.ApiService.SendPostRequest<Message>($"chats/{chatId}/chat_messages", message);
                if (createdMessage != null) {
                   
                    CurrentChatData.ChatMessages.Add(createdMessage);
                    CurrentChatData.Init(Users, CurrentUser.Id);
                }
                IsSentMessageEnabled = true;
                //ReadAllNewMessages();
                await Task.CompletedTask;
            }
        }

        public List<Message> ReadAllNewMessages()
        {
            var messages = Task.Run(async () => await GetNewMessages()).Result;
            if (messages != null && CurrentChatData!=null)
            {
                messages.ForEach(e => CurrentChatData.ChatMessages.Add(e));
                CurrentChatData.Init(Users, CurrentUser.Id);
                return messages;
            }
            return new List<Message>();
        }

        public void StartMonitoringLoop()
        {
            if (!_IsMonitoringRunning && CurrentChatData != null)
            {
                _IsMonitoringRunning = true;
                //Task.Run(MonitoringLoop);
                var newMessages = Task.Run(GetNewMessages);
                foreach (var message in newMessages.Result)
                {
                    CurrentChatData.ChatMessages.Add(message);
                }
                //Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //{
                //    MonitoringLoop();
                //});
            }
        }

        public async Task<List<Message>> GetNewMessages()
        {
            var newMessages = new List<Message>();
            System.Diagnostics.Debug.WriteLine("Started");
            while (true)
            {
                await Task.Delay(1000);
                if (CurrentChatData != null)
                {
                    try
                    {
                        var chatId = CurrentChatData.Id;
                        var partnerId = CurrentChatData.PartnerId;
                        var oponentsMessagesCount = CurrentChatData.ChatMessages.Where(m => m.UserSentId == partnerId).Count();
                        var query = $"chats/{chatId}/chat_messages?user_sent_id={partnerId}";//&sortBy=id&order=desc

                        var partnerMessagesFromApi = AppDataService.ApiService.ReadAll<Message>(query).ToList();
                        if (partnerMessagesFromApi != null && partnerMessagesFromApi.Count > 0)
                        {
                            var newMessagesCount = partnerMessagesFromApi.Count - oponentsMessagesCount;
                            newMessages = partnerMessagesFromApi.GetRange(oponentsMessagesCount, newMessagesCount);
                            if (newMessages.Count > 0)
                            {
                                return newMessages;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine($"error: {e.Message}");
                        throw;
                    }
                }
            }
        }
        public void UpdateChat(object? sender, EventArgs? e)
        {
            var oldChats = UserChats.ToList();
            var apiChats = AppDataService.SetUserChatsFromApi(CurrentUser.Id);
            var newChats = apiChats.Where(chat => !oldChats.Select(x => x.Id).Contains(chat.Id)).ToList();
            newChats.ForEach(chat =>
            {
                chat.Init(Users, CurrentUser.Id);
                var newMessagesCount = chat.ChatMessages.Count;
                chat.NewMessagesCount = newMessagesCount;
                chat.IsNewMessages = true;
                if (chat.PartnerId != CreatingChatUserId)
                {
                    UserChats.Insert(0, chat);
                }
            });
            foreach (var chat in oldChats)
            {
                var apiChatData = apiChats.Find(c=>c.Id == chat.Id);
                if (apiChatData != null && apiChatData.ChatMessages.Count > chat.ChatMessages.Count)
                {
                    apiChatData.EncriptingTextKey = chat.EncriptingTextKey;
                    apiChatData.Init(Users, CurrentUser.Id);
                    var chatIndex = UserChats.ToList().FindIndex(c=>c.Id==chat.Id);
                    if (chatIndex != -1)
                    {
                        if (CurrentChatData != null && UserChats[chatIndex].Id == CurrentChatData.Id)
                        {
                            var newOponentMessages = apiChatData.ChatMessages
                                .Where(mess => mess.UserSentId != CurrentUser.Id && !CurrentChatData.ChatMessages.Select(m => m.Id).Contains(mess.Id)).ToList();
                            newOponentMessages.ForEach(m => CurrentChatData.ChatMessages.Add(m));
                        } else
                        {
                            var newMessagesCount = apiChatData.ChatMessages.Count + chat.NewMessagesCount - chat.ChatMessages.Count;
                            apiChatData.newMessagesCount = newMessagesCount;
                            apiChatData.IsNewMessages = true;
                            UserChats[chatIndex] = apiChatData;
                        }
                    }
                    
                }
            }
        }

        public void StartUpdateTimer(bool IsStop = false)
        {
            UpdateTimer.Stop();
            UpdateTimer.Tick -= new EventHandler(UpdateChat);
            if (!IsStop)
            {
                UpdateTimer.Tick += new EventHandler(UpdateChat);
                UpdateTimer.Interval = TimeSpan.FromSeconds(5);
                UpdateTimer.Start();
            }
            
        }


    }
}