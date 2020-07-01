using System.Net;
using MusicSDK.Netease.Models;

namespace MusicSDK.Netease
{
    public class Session
    {
        public Session(User user, CookieContainer cookieJar)
        {
            User = user;
            CookieJar = cookieJar;
        }

        public User User { get; set; }
        
        public CookieContainer CookieJar { get; set; }
    }

}