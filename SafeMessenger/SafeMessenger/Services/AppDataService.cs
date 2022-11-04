using SafeMessenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeMessenge.Services;

public class AppDataService
{
    public ApiService ApiService;
    public List<User> Users { get; set; } = new();
    public User? CurrentUser { get; set; }
    public List<Chat> Chats { get; set; } = new();

    public AppDataService(ApiService apiService)
    {
        ApiService = apiService;

    }

    public List<User> SetUsersFromApi()
    {
        Users = ApiService.ReadAll<User>("users").ToList();
        return Users;
    }

    public List<Chat> SetUserChatsFromApi(long userId)
    {
        var chats = ApiService.ReadAll<Chat>($"chats?users_id_array={userId}").ToList();
        Chats = chats.Where(chat => chat.UsersIds.Contains(userId)).ToList();
        return Chats;
    }
}
