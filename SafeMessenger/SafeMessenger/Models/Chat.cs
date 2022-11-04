using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using SafeMessenge.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SafeMessenge.Models;
[JsonObject]
public partial class Chat : ObservableRecipient, IResponseModel
{
    [JsonProperty("id")]
    public long Id;
    [JsonProperty("users_id_array")]
    public List<long> UsersIds;
    [JsonProperty("chat_messages")]
    public ObservableCollection<Message> ChatMessages;
    public User Partner = new();
    public long PartnerId = new();
    [ObservableProperty]
    public string _EncriptingTextKey = "";
    [ObservableProperty]
    public int newMessagesCount = 0;  
    [ObservableProperty]
    public bool _IsNewMessages = false;
    public void DecodeMessages()
    {
        ChatMessages = ChatMessages ?? new(new List<Message>());
        foreach (var message in ChatMessages)
        {
            var chiper = new RC4Helper(EncriptingTextKey);
            var decriptedMessage = chiper.DectyptStringMessage(message.EncryptedMessage);
            message.DecryptedMessage = decriptedMessage;
        }
    }

    public long SetPartnerId(long userId)
    {
        var partnerId = UsersIds.ToList().Find(x => x != userId);
        PartnerId = partnerId;
        return partnerId;
    }

    public Chat Init(List<User> currentUsers, long myUserId)
    {
        var partner  = currentUsers.Where(x => x.Id == SetPartnerId(myUserId)).First();
        Partner = partner;
        ChatMessages = ChatMessages ?? new(new List<Message>());
        foreach (var message in ChatMessages)
        {
            message.UserSent = currentUsers.Find(u => u.Id == message.UserSentId) ?? new() { Name = "undefined user"};
            message.Alignment = message.UserSent.Id == myUserId ? "Right" : "Left";
        }
        DecodeMessages();
        return this;
    }
}
[JsonObject]
[ObservableObject]
public partial class Message: IResponseModel
{
    [JsonProperty("id")]
    public long Id;
    [JsonProperty("chat_id")]
    public long ChatId;
    [JsonProperty("sent_date")]
    public DateTime SentDate;
    [JsonProperty("encrypted_message")]
    public string EncryptedMessage;
    [ObservableProperty]
    public string decryptedMessage ="";
    [JsonProperty("message_type")]
    public string MessageType;
    [JsonProperty("user_sent_id")]
    public long UserSentId;
    public User UserSent;
    public string Alignment;

    public HorizontalAlignment GetOppositeAlignment()
    {
        var isRight = Alignment == "Right";
        var alignment = isRight? HorizontalAlignment.Left : HorizontalAlignment.Right;
        return alignment;
    }

}