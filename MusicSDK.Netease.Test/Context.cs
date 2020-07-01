using System.Threading.Tasks;
using MusicSDK.Netease.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MusicSDK.Netease.Test
{
    [TestClass]
    public class Context
    {
        public static NeteaseMusicSDK Api;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Api = new NeteaseMusicSDK(new Storage());
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Api.Dispose();
        }
        
        public static async Task<User> EnsureLogined()
        {
            if (Api.Me == null)
            {
                await Api.LoginAsync(Configuration.Username, Configuration.Password);
            }
            return Api.Me;
        }
    }
}

