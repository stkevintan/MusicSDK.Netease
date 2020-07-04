using System.Collections.Generic;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicSDK.Netease.Models;
using System.Threading.Tasks;

namespace MusicSDK.Netease.Test
{
    [TestClass]
    public class TestApi
    {
        [TestMethod]
        public async Task ShouldSearchSongWork()
        {
            var ret = await Context.Api.SearchAsync<Song>("A");
            Assert.IsNotNull(ret.Items);
        }

        [TestMethod]
        public async Task ShouldSearchAlbumWork()
        {
            var ret = await Context.Api.SearchAsync<Album>("A");
            Assert.IsNotNull(ret.Items);
        }

        [TestMethod]
        public async Task ShouldSearchArtistWork()
        {
            var ret = await Context.Api.SearchAsync<Artist>("A");
            Assert.IsNotNull(ret.Items);
        }

        [TestMethod]
        public async Task ShouldSearchPlaylistWork()
        {
            var ret = await Context.Api.SearchAsync<Playlist>("A");
            Assert.IsNotNull(ret.Items);
        }

        [TestMethod]
        public async Task ShouldSearchUserWork()
        {
            var ret = await Context.Api.SearchAsync<User>("A");
            Assert.IsNotNull(ret.Items);
        }

        [TestMethod]
        public async Task ShouldDailyTaskWork()
        {
            await Context.EnsureLogined();
            try
            {
                var point = await Context.Api.DailyTaskAsync();
                Assert.IsTrue(point > 0);
            }
            catch (HttpRequestException e)
            {
                Assert.AreEqual(e.Data["Code"], -2);
            }
        }

        [TestMethod]
        public async Task ShouldRecommendWork()
        {
            await Context.EnsureLogined();
            await Context.Api.RecommendAsync<Playlist>();
            await Context.Api.RecommendAsync<Song>();
        }

        [TestMethod]
        public async Task ShouldRecommendNotWorkWithoutLogin()
        {
            var api = new NeteaseMusicSDK();
            await Assert.ThrowsExceptionAsync<HttpRequestException>(api.RecommendAsync<Playlist>);
            api.Dispose();
        }

        [TestMethod]
        public async Task ShouldUserPlaylistWork()
        {
            var user = await Context.EnsureLogined();
            var plist = await Context.Api.UserPlaylistAsync(user.Id);
        }

        [TestMethod]
        public async Task ShouldPersonalFmWork()
        {
            await Context.EnsureLogined();
            var ret = await Context.Api.PersonalFmAsync();
            Assert.IsTrue(ret.Count > 0);
        }

        [TestMethod]
        public async Task ShouldSongUrlWork()
        {
            // await Context.EnsureLogined();
            // ALiz: 29307041
            var ret = await Context.Api.SongUrlAsync(29307041);
            var ret2 = await Context.Api.SongUrlAsync(29307041, (int)SongQuality.HD);
            Assert.AreEqual(ret.Quality, ret2.Quality);
            Assert.IsNull(ret.FreeTrialInfo);

            var ret3 = await Context.Api.SongUrlAsync(new List<long>() { 29307041, 29829683 });
            Assert.IsTrue(ret3 is List<SongUrl>);

            //vip
            var ret4 = await Context.Api.SongUrlAsync(26127499);
            Assert.IsNotNull(ret4.FreeTrialInfo);

        }

        [TestMethod]
        public async Task ShouldSongLyricWork()
        {
            // await Context.EnsureLogined();
            var ret = await Context.Api.SongLyricAsync(29829683);
        }

        [TestMethod]
        public async Task ShouldSongDetailWork()
        {
            var ret = await Context.Api.SongDetailAsync(29829683);
            Assert.IsNotNull(ret);
            var rlist = await Context.Api.SongDetailAsync(new List<long>() { 29829683, 29307041 });
            Assert.IsTrue(rlist.Count == 2);
        }

        [TestMethod]
        public async Task ShouldAlbumDetailWork()
        {
            var ret = await Context.Api.AlbumDetailAsync(3086101);
            Assert.IsTrue(ret.Songs.Count > 0);
        }

        [TestMethod]
        public async Task ShouldArtistAlbumWork()
        {
            var ret = await Context.Api.ArtistAlbumAsync(15988, 0, 2);
            Assert.IsTrue(ret.Count > 0);
        }

        [TestMethod]
        public async Task ShouldArtistDescWork()
        {
            await Context.Api.ArtistDescAsync(15988);
        }

        [TestMethod]
        public async Task ShouldArtistTopSongsWork()
        {
            await Context.Api.ArtistTopSongsAsync(15988);
        }

        [TestMethod]
        public async Task ShouldPlaylistDetailWork()
        {
            await Context.EnsureLogined();
            await Context.Api.PlaylistDetailAsync(24795857);
            // {"code":200,"relatedVideos":null,"playlist":{"subscribers":[],"subscribed":false,"creator":null,"tracks":[],"trackIds":[],"updateFrequency":null,"backgroundCoverId":0,"backgroundCoverUrl":null,"titleImage":0,"titleImageUrl":null,"englishTitle":null,"opRecommend":false,"ordered":false,"trackNumberUpdateTime":0,"adType":0,"description":null,"status":0,"tags":[],"subscribedCount":0,"cloudTrackCount":0,"userId":40027322,"createTime":1413805008607,"highQuality":false,"coverImgUrl":"https://p4.music.126.net/EWC8bPR8WW9KvhaftdmsXQ==/3397490930543093.jpg","coverImgId":3397490930543093,"specialType":5,"updateTime":1413805008607,"commentThreadId":"A_PL_0_33287225","privacy":0,"trackUpdateTime":1413805013226,"trackCount":0,"newImported":false,"playCount":0,"name":"我喜欢的音乐","id":33287225,"shareCount":0,"commentCount":0},"urls":null,"privileges":null}
        }

        [TestMethod]
        public async Task ShouldResourceExposureWork()
        {
            await Context.Api.ResourceExposure(492076397);
        }
    }
}
