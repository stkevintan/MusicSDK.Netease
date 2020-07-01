using System.Collections.Generic;
using MusicSDK.Netease.Library;

#nullable disable
namespace MusicSDK.Netease.Models
{
    [EntityInfo(100)]
    public class Artist : BaseModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<string> Alias { get; set; }

        public string PicUrl { get; set; }

        public int AlbumSize { get; set; }

        public List<string> Tns { get; set; }
    }
}