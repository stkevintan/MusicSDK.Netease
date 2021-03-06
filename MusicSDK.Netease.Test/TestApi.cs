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
            var ret = await Context.Api.SearchAsync<Song>("灰色と青");
            Assert.IsNotNull(ret.Items);
        }

        [TestMethod]
        public async Task ShouldSearchAlbumWork()
        {
            var ret = await Context.Api.SearchAsync<Album>("황진이 OST");
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
            await Context.Api.UserPlaylistAsync(user.Id);
        }

        [TestMethod]
        public async Task ShouldPersonalRadioWork()
        {
            await Context.EnsureLogined();
            var ret = await Context.Api.PersonalRadioAsync();
            Assert.IsTrue(ret.Count > 0);
            Assert.IsNotNull(ret[0].Album);
            Assert.IsNotNull(ret[0].Artists?[0]);
        }

        [TestMethod]
        public async Task ShouldSongUrlWork()
        {
            // await Context.EnsureLogined();
            // ALiz: 29307041
            var ret = await Context.Api.SongUrlAsync(29307041);
            var ret2 = await Context.Api.SongUrlAsync(29307041, (int)SongQuality.HQ);
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
            Assert.IsFalse(ret.Nolyric);
            Assert.IsNotNull(ret.Lrc.lyric);

            var ret1 = await Context.Api.SongLyricAsync(442457);
            Assert.IsTrue(ret1.Nolyric);
            Assert.IsNull(ret1.Lrc);
        }

        [TestMethod]
        public async Task ShouldSongDetailWork()
        {
            var ret = await Context.Api.SongDetailAsync(442454);
            Assert.IsNotNull(ret);
            var rlist = await Context.Api.SongDetailAsync(new List<long>() { 29829683, 29307041 });
            Assert.IsTrue(rlist.Count == 2);
        }

        [TestMethod]
        public async Task ShouldAlbumDetailWork()
        {
            var (album, songs) = await Context.Api.AlbumDetailAsync(36529043);
            Assert.IsNotNull(album);
            Assert.IsTrue(songs.Count > 0);
            // Privilege should not avaliable
            Assert.IsNull(songs[0].Privilege);
        }

        [TestMethod]
        public async Task ShouldAlbumPrivilegeWork()
        {
            var privilege = await Context.Api.AlbumPrivilegeAsync(36529043);
            Assert.IsNotNull(privilege);
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
        }

        [TestMethod]
        public async Task ShouldResourceExposureWork()
        {
            await Context.Api.ResourceExposure(492076397);
        }
    }
}
