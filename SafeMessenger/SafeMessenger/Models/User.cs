using Newtonsoft.Json;

namespace SafeMessenge.Models;

[JsonObject]
public class User : IResponseModel
{
    [JsonProperty("id")]
    public long Id;
    [JsonProperty("name")]
    public string Name;
    [JsonProperty("password")]
    public string Password;
    public string Avatar => $"https://dummyimage.com/400x400/000000/0011ff&text=user{Id}";
    //private string _avatar = "";
    //public string Avatar { get { return _avatar; } set { _avatar = $"https://dummyimage.com/400x400/000000/0011ff&text=user{Id}"; } }
    //public User()
    //{

    //}
    //public User(long id, string name, string password)
    //{
    //    Id = id;
    //    Name = name;
    //    Password = password;
    //    Avatar = $"https://dummyimage.com/400x400/000000/0011ff&text=user+{Id}";
    //}
}
