using System.Collections.Generic;
using System.Threading.Tasks;
using MusicSDK.Netease.Models;

namespace MusicSDK.Netease
{
    public interface INeteaseMusicSDK
    {
        /// <summary>
        /// Login with cellphone and password, grab account cookies for later api calls
        /// </summary>
        /// <param name="username">Only cellphone number is support</param>
        /// <param name="password"></param>
        /// <returns>The login user</returns>
        Task<User> LoginAsync(string username, string password);

        /// <summary>
        /// Refresh the login token
        /// </summary>
        Task LoginRefreshAsync();

        /// <summary>
        /// Search Songs, Playlists, Artists, Albums and Users
        /// </summary>
        /// <param name="keyword">Search keyword</param>
        /// <param name="offset">Query offset, default: 0</param>
        /// <param name="total"></param>
        /// <param name="limit">Query limit, default: 50</param>
        /// <typeparam name="T">Only `Song`, `Playlist`, `Artist`, `Album` and `User` are supported</typeparam>
        /// <returns>Result of search</returns>
        Task<ListResult<T>> SearchAsync<T>(string keyword, int offset = 0, bool total = true, int limit = 50) where T : IBaseModel;

        /// <summary>
        /// Do the daily task
        /// </summary>
        /// <param name="isMobile">if it is mobile client, default: true</param>
        /// <returns>Points your got, or an exception when you check more than once in one day.</returns>
        Task<int> DailyTaskAsync(bool isMobile = true);

        /// <summary>
        /// Get user's playlists
        /// </summary>
        /// <param name="Id">User's Id</param>
        /// <param name="offset">Query offset, default: 50</param>
        /// <param name="limit">Query limit, default: 0</param>
        /// <returns>Playlists</returns>
        Task<List<Playlist>> UserPlaylistAsync(long Id, int offset = 0, int limit = 50);

        /// <summary>
        /// Get recommended playlists or songs
        /// </summary>
        /// <typeparam name="T">Only Playlist and Song are valid</typeparam>
        /// <returns>Playlists or Songs</returns>
        Task<List<T>> RecommendAsync<T>() where T : IBaseModel;

        /// <summary>
        /// Get 3 personal fm songs
        /// </summary>
        /// <returns>Songs</returns>
        Task<List<Song>> PersonalRadioAsync();

        /// <summary>
        /// Get a song's audio url
        /// </summary>
        /// <param name="Id">Song's id</param>
        /// <param name="br">Audio quality, HD: 320000,  MD: 192000, LD: 128000</param>
        /// <returns>Song's audio url</returns>

        Task<SongUrl> SongUrlAsync(long Id, int br = 999000);

        /// <summary>
        /// Batch get songs' audio urls
        /// </summary>
        /// <param name="IdList">Songs' id list</param>
        /// <param name="br">Audio quality, HD: 320000,  MD: 192000, LD: 128000</param>
        /// <returns>Songs' audio urls</returns>
        Task<List<SongUrl>> SongUrlAsync(List<long> IdList, int br = 999000);

        /// <summary>
        /// Get a song's lyric
        /// </summary>
        /// <param name="Id">Song's id</param>
        /// <returns>Song's lyric</returns>
        Task<SongLyric> SongLyricAsync(long Id);

        /// <summary>
        /// Get a single song's detail
        /// </summary>
        /// <param name="Id">Song's id</param>
        /// <returns>Song's detail</returns>
        Task<Song> SongDetailAsync(long Id);

        /// <summary>
        /// Batch get songs' details
        /// </summary>
        /// <param name="Ids">Songs' id list</param>
        /// <returns>Songs' detail</returns>
        Task<List<Song>> SongDetailAsync(List<long> Ids);

        /// <summary>
        /// Get album's details
        /// </summary>
        /// <param name="Id">Album's id</param>
        /// <returns>Album</returns>
        Task<(Album, List<Song>)> AlbumDetailAsync(long Id);

        /// <summary>
        /// Artist's Album list
        /// </summary>
        /// <param name="Id">Album's id</param>
        /// <param name="offset">Query offset, default: 0</param>
        /// <param name="limit">Query limit, default 50</param>
        /// <returns>Artist's albums</returns>
        Task<List<Album>> ArtistAlbumAsync(long Id, int offset = 0, int limit = 30);

        /// <summary>
        /// Artist Description
        /// </summary>
        /// <param name="Id">Artist id</param>
        /// <returns>Artist's descriptions</returns>
        Task<ArtistDesc> ArtistDescAsync(long Id);

        /// <summary>
        /// Hot 50 songs for a artist
        /// </summary>
        /// <param name="Id">Artist's id</param>
        /// <returns>Song's details</returns>
        Task<List<Song>> ArtistTopSongsAsync(long Id);
        Task<(PlaylistDetail, List<Privilege>)> PlaylistDetailAsync(long Id);
        Task ResourceExposure(long Id);
        Task<List<Privilege>> AlbumPrivilegeAsync(long Id, int offset = 0, int limit = 500);
    }
}