using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MusicSDK.Netease.Library;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable disable
namespace MusicSDK.Netease.Models
{
    public class ArtistBase : IBaseModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<string> Alias { get; set; }

        [JsonProperty(PropertyName = "tns")]
        public List<string> TransNames { get; set; }
    }

    [EntityInfo(100)]
    public class Artist : ArtistBase
    {
        public string PicUrl { get; set; }

        public int AlbumSize { get; set; }
    }
}