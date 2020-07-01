using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicSDK.Netease.Models;
using System.Threading.Tasks;

namespace MusicSDK.Netease.Test
{
    [TestClass]
    public class TestLogin
    {
        [TestMethod]
        public async Task ShouldCellphoneLoginSuccess()
        {
            var api = new NeteaseMusicSDK();
            var user = await api.LoginAsync(Configuration.Username, Configuration.Password);
            Assert.AreEqual(user.Name, Configuration.Nickname);
        }

        [TestMethod]
        public async Task ShouldLoginRefreshSuccess()
        {
            await Context.EnsureLogined();
            await Context.Api.LoginRefreshAsync();
        }

        [TestMethod]
        public async Task ShouldCookiePersist()
        {
            var storage = new Storage();
            var api = new NeteaseMusicSDK(storage);
            var user = api.Me ?? await api.LoginAsync(Configuration.Username, Configuration.Password);
            api.Dispose();
            api = new NeteaseMusicSDK(storage);
            Assert.IsNotNull(api.Me);
            await api.RecommendAsync<Song>();
        }

        [TestMethod]
        public async Task ShouldCookieMaintainered()
        {
            await Context.EnsureLogined();
            var ret = await Context.Api.SearchAsync<Song>("A");
        }
    }
}