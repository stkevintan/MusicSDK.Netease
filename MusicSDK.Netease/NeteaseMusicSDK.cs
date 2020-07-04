using System.Reflection;
using System;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using MusicSDK.Netease.Models;
using MusicSDK.Netease.Library;


namespace MusicSDK.Netease
{
    public class NeteaseMusicSDK : INeteaseMusicSDK, IDisposable
    {
        readonly CookieContainer cookieJar;
        readonly IStorage? storage;
        readonly List<RequestProvider> requestProviders = new List<RequestProvider>();

        public User? Me { get; private set; }

        /// <summary>
        /// IStorage is an optional parameters which can persist the login account and cookies to it storage implement
        /// </summary>
        /// <param name="storage"></param>
        public NeteaseMusicSDK(IStorage? storage = null)
        {
            this.storage = storage;
            var session = storage?.LoadSession();
            if (session != null)
            {
                Me = session.User;
                cookieJar = session.CookieJar;
            }
            else
            {
                cookieJar = new CookieContainer();
            }
            InitRequestProviders();

            CheckSession();
        }
        void InitRequestProviders()
        {
            var rType = typeof(RequestProvider);
            var assembly = Assembly.GetAssembly(rType)!;
            var list = from t in assembly.GetExportedTypes() where rType.IsAssignableFrom(t) && !t.IsAbstract select t;
            foreach (var t in list)
            {
                requestProviders.Add((RequestProvider)Activator.CreateInstance(t, cookieJar)!);
            }
        }
        void CheckSession()
        {
            foreach (var rp in requestProviders)
            {
                var cookies = cookieJar.GetCookies(rp.PublicUri);
                rp.InitCookies();
                // if cookie is expired, clear the login status.
                var count = (from c in cookies where c.Expired select c).Count();
                if (count > 0)
                {
                    cookies.Clear();
                    Me = null;
                    storage?.ClearSession();
                    return;
                }
            }
        }

        public void Dispose()
        {
            if (Me != null)
            {
                storage?.SaveSession(new Session(Me, cookieJar));
            }
            else
            {
                storage?.ClearSession();
            }
            requestProviders.ForEach(r => r.Dispose());
        }

        ResponseResolver Request(
           string path,
           Dictionary<string, object>? body = null,
           HttpMethod? method = null)
        {
            var provider = (from r in requestProviders where r.Match(path) select r).Single();
            return new ResponseResolver(provider.RequestAsync(path, body, method));
        }

        #region API
        public async Task<User> LoginAsync(string username, string password)
        {
            var url = "/weapi/login/cellphone";
            var body = new Dictionary<string, object>
            {
                {"phone", username},
                {"password", Encrypt.Md5(password)},
                {"rememberLogin", "true"}
            };
            var res = await Request(url, body).Into(new { Profile = default(User) });
            return Me = Utility.AssertNotNull(res?.Profile);
        }

        public async Task LoginRefreshAsync()
        {
            var url = "/weapi/login/token/refresh";
            await Request(url).Into();
        }

        public async Task LogoutAsync()
        {
            var url = "/weapi/logout";
            await Request(url).Into();
            storage?.ClearSession();
            Me = null;
        }

        public async Task<ListResult<T>> SearchAsync<T>(string keyword, int offset = 0, bool total = true, int limit = 50) where T : IBaseModel
        {
            var url = "/eapi/cloudsearch/pc";
            var type = typeof(T);
            EntityInfoAttribute? entityInfo = (EntityInfoAttribute?)Attribute.GetCustomAttribute(type, typeof(EntityInfoAttribute));
            if (entityInfo == null)
            {
                throw new NotSupportedException($"Cannot search type {type.Name}, Only {nameof(Song)}, {nameof(Album)}, {nameof(Artist)} {nameof(User)} and {nameof(Playlist)} are supported.");
            }
            var typeName = entityInfo.Name ?? type.Name;

            var body = new Dictionary<string, object> {
                {"s", keyword},
                {"type", entityInfo.Type},
                {"offset", offset},
                {"total", total ? "true":"false"},
                {"limit", limit}
            };
            var res = await Request(url, body).Into(
                new { Result = default(ListResult<T>) },
                new JsonSerializerSettings
                {
                    ContractResolver = new DynamicContractResolver
                    {
                        Renames = new Dictionary<string, string> {
                            {"Items",  typeName + "s"},
                            {"Count", typeName + "Count"}
                        }
                    }
                });

            var result = Utility.AssertNotNull(res.Result);
            result.Count = result.Count == 0 ? result.Items.Count : result.Count;
            return result;
        }

        public async Task<int> DailyTaskAsync(bool isMobile = true)
        {
            var path = "/weapi/point/dailyTask";
            var body = new Dictionary<string, object> { { "type", isMobile ? 0 : 1 } };
            var ret = await Request(path, body).Into(new { Point = default(int) });
            return ret.Point;
        }

        public async Task<List<Playlist>> UserPlaylistAsync(long Id, int offset = 0, int limit = 1000)
        {
            var path = "/eapi/user/playlist";
            var body = new Dictionary<string, object> {
                {"uid", Id},
                {"offset", offset},
                {"limit", limit},
            };
            var ret = await Request(path, body).Into(new { Playlist = default(List<Playlist>) });
            return Utility.AssertNotNull(ret.Playlist);
        }

        public async Task<List<T>> RecommendAsync<T>() where T : IBaseModel
        {
            var type = typeof(T);
            var path = "/weapi/v1/discovery/recommend";
            if (type.Name == "Playlist")
            {
                path += "/resource";
            }
            else if (type.Name == "Song")
            {
                path += "/songs";
            }
            else
            {
                throw new NotSupportedException($"Recommend {type.Name} is not supported");
            }
            var ret = await Request(path).Into(new { Recommend = default(List<T>) });
            return Utility.AssertNotNull(ret.Recommend);
        }

        public async Task<List<Song>> PersonalRadioAsync()
        {
            var path = "/eapi/v1/radio/get";
            var ret = await Request(path).Into(new { Data = default(List<SongLegacy>) });
            return Utility.AssertNotNull(ret?.Data.Select(x => (Song)x).ToList());
        }

        public async Task<SongUrl> SongUrlAsync(long Id, int br = (int)SongQuality.SQ)
        {
            var path = "/eapi/song/enhance/player/url";
            var body = new Dictionary<string, object> {
                {"ids", new long[] {Id}},
                {"br", br}
            };
            var ret = await Request(path, body).Into(new { Data = default(List<SongUrl>) });
            return Utility.AssertNotNull(ret.Data?.FirstOrDefault());
        }

        public async Task<List<SongUrl>> SongUrlAsync(List<long> IdList, int br = (int)SongQuality.SQ)
        {
            var path = "/eapi/song/enhance/player/url";
            var body = new Dictionary<string, object> {
                {"ids", IdList},
                {"br", br}
            };
            var ret = await Request(path, body).Into(new { Data = default(List<SongUrl>) });
            return Utility.AssertNotNull(ret.Data);
        }

        public async Task<SongLyric> SongLyricAsync(long Id)
        {
            var path = "/eapi/song/lyric";
            var body = new Dictionary<string, object> {
                {"os", "pc"},
                {"id", Id},
                {"lv", -1},
                {"kv", -1},
                {"tv", -1},
            };
            var ret = await Request(path, body).Into<SongLyric>();
            return ret;
        }

        public async Task<Song> SongDetailAsync(long Id)
        {
            // lack of privilege info.
            var path = "/weapi/v3/song/detail";
            var body = new Dictionary<string, object> {
                {"c", $"[{{\"id\": {Id}}}]"},
                {"ids", $"[{Id}]"}
            };
            var ret = await Request(path, body).Into(new { Songs = default(List<Song>) });
            return Utility.AssertNotNull(ret.Songs?.FirstOrDefault());
        }

        public async Task<List<Song>> SongDetailAsync(List<long> Ids)
        {
            // lack of privilege info.
            var path = "/weapi/v3/song/detail";
            var body = new Dictionary<string, object> {
                {"c", $"[{String.Join(',', Ids.Select(id => $"{{\"id\": {id}}}"))}]"},
                {"ids", $"[{String.Join(',', Ids)}]"}
            };
            var ret = await Request(path, body).Into(new { Songs = default(List<Song>) });
            return Utility.AssertNotNull(ret.Songs);
        }

        public async Task<(Album, List<Song>)> AlbumDetailAsync(long Id)
        {
            var path = "/eapi/album/v3/detail";
            var body = new Dictionary<string, object>
            {
                {"id", Id }
            };
            // Song will lack the privilege property.
            var ret = await Request(path, body).Into(new { Album = default(Album), Songs = default(List<Song>) });
            return (Utility.AssertNotNull(ret.Album), Utility.AssertNotNull(ret.Songs));
        }

        public async Task<List<Privilege>> AlbumPrivilegeAsync(long Id, int offset = 0, int limit = 500)
        {
            var path = "/eapi/album/privilege";
            var body = new Dictionary<string, object>
            {
                {"id", Id },
                {"offset", offset },
                {"total", "true" },
                { "limit", limit }
            };
            var ret = await Request(path, body).Into(new { Data = default(List<Privilege>) });
            return Utility.AssertNotNull(ret.Data);
        }

        public async Task<List<Album>> ArtistAlbumAsync(long Id, int offset = 0, int limit = 30)
        {
            var path = $"/weapi/artist/albums/{Id}";
            var body = new Dictionary<string, object>
            {
                ["limit"] = limit,
                ["offset"] = offset,
                ["total"] = true
            };
            var ret = await Request(path, body).Into(new { HotAlbums = default(List<Album>) });
            return Utility.AssertNotNull(ret?.HotAlbums);
        }

        public async Task<ArtistDesc> ArtistDescAsync(long Id)
        {
            var path = $"/weapi/artist/introduction";
            var body = new Dictionary<string, object> {
                {"id", Id}
            };
            var ret = await Request(path, body).Into<ArtistDesc>();
            return Utility.AssertNotNull(ret);
        }

        public async Task<List<Song>> ArtistTopSongsAsync(long Id)
        {
            var path = "/weapi/artist/top/song";
            var body = new Dictionary<string, object> {
                {"id", Id}
            };
            var ret = await Request(path, body).Into(new { Songs = default(List<Song>) });
            return Utility.AssertNotNull(ret?.Songs);
        }


        public async Task<(PlaylistDetail, List<Privilege>)> PlaylistDetailAsync(long Id)
        {
            var path = "/eapi/v6/playlist/detail";
            var body = new Dictionary<string, object>
            {
                {"id", Id},
                {"t", -1 },
                {"n", 500},
                {"s", 0 }
            };
            var ret = await Request(path, body).Into(new
            {
                Playlist = default(PlaylistDetail),
                Privileges = default(List<Privilege>)
            });
            return (Utility.AssertNotNull(ret?.Playlist), Utility.AssertNotNull(ret?.Privileges));
        }


        /// <summary>
        /// unknown usage
        /// </summary>
        public async Task ResourceExposure(long Id)
        {
            var path = "/eapi/resource-exposure/config";
            var body = new Dictionary<string, object>
            {
                {"resourcePosition", "playlist" },
                {"resourceId", Id }
            };
            _ = await Request(path, body).Into();
        }
        #endregion API
    }
}