using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MusicSDK.Netease.Library;
/*
{
  "songs": [
    {
      "name": "小さな恋のうた",
      "id": 29829683,
      "pst": 0,
      "t": 0,
      "ar": [
        {
          "id": 15988,
          "name": "新垣結衣",
          "tns": [],
          "alias": []
        }
      ],
      "alia": [],
      "pop": 100,
      "st": 0,
      "rt": null,
      "fee": 0,
      "v": 56,
      "crbt": null,
      "cf": "",
      "al": {
        "id": 3086101,
        "name": "小さな恋のうた",
        "picUrl": "http://p4.music.126.net/6zzWtkMnCAD4yxk_r4CUJA==/2528876746537476.jpg",
        "tns": [],
        "pic": 2528876746537476
      },
      "dt": 326000,
      "h": {
        "br": 320000,
        "fid": 0,
        "size": 13043629,
        "vd": -22500
      },
      "m": {
        "br": 192000,
        "fid": 0,
        "size": 7826244,
        "vd": -20200
      },
      "l": {
        "br": 128000,
        "fid": 0,
        "size": 5217552,
        "vd": -18700
      },
      "a": null,
      "cd": "1",
      "no": 1,
      "rtUrl": null,
      "ftype": 0,
      "rtUrls": [],
      "djId": 0,
      "copyright": 0,
      "s_id": 0,
      "mark": 9007199255003136,
      "originCoverType": 0,
      "single": 0,
      "noCopyrightRcmd": null,
      "mv": 5347593,
      "mst": 9,
      "cp": 663018,
      "rtype": 0,
      "rurl": null,
      "publishTime": 1257436800007,
      "tns": [
        "小小恋歌"
      ]
    }
  ],
  "privileges": [
    {
      "id": 29829683,
      "fee": 0,
      "payed": 0,
      "st": 0,
      "pl": 320000,
      "dl": 320000,
      "sp": 7,
      "cp": 1,
      "subp": 1,
      "cs": false,
      "maxbr": 320000,
      "fl": 320000,
      "toast": false,
      "flag": 0,
      "preSell": false,
      "playMaxbr": 320000,
      "downloadMaxbr": 320000
    }
  ],
  "code": 200
}
*/
#nullable disable
namespace MusicSDK.Netease.Models
{
    [EntityInfo(1)]
    public class Song : IBaseModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        [JsonProperty(PropertyName = "al")]
        public virtual AlbumBase Album { get; set; }

        [JsonProperty(PropertyName = "alia")]
        public virtual List<string> Alias { get; set; }

        [JsonProperty(PropertyName = "ar")]
        public virtual List<ArtistBase> Artists { get; set; }

        [JsonProperty(PropertyName = "mv")]
        public virtual long MvId { get; set; }

        /// <summary>
        /// Privilege info, Not always exist
        /// </summary>
        public Privilege Privilege { get; set; }

        [JsonProperty(PropertyName = "tns")]
        public virtual List<string> TransNames { get; set; }

        [JsonExtensionData]
        private readonly IDictionary<string, JToken> _raw = new Dictionary<string, JToken>();


        public List<SongQuality> Qualities { get; set; } = new List<SongQuality>();


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_raw.ContainsKey("h")) Qualities.Add(SongQuality.HQ);
            if (_raw.ContainsKey("m")) Qualities.Add(SongQuality.MQ);
            if (_raw.ContainsKey("l")) Qualities.Add(SongQuality.LQ);
        }

        [JsonConverter(typeof(DateConverter))]
        public DateTime PublishTime { get; set; }
    }

    public class SongLegacy : Song
    {
        [JsonProperty(PropertyName = "mvid")]
        public override long MvId { get; set; }

        [JsonProperty(PropertyName = "album")]
        public override AlbumBase Album { get; set; }

        [JsonProperty(PropertyName = "alias")]
        public override List<string> Alias { get; set; }

        [JsonProperty(PropertyName = "artists")]
        public override List<ArtistBase> Artists { get; set; }

        [JsonProperty(PropertyName = "transNames")]
        public override List<string> TransNames { get; set; }

        public int Duration { get; set; }

        [JsonExtensionData]
        private readonly IDictionary<string, JToken> _raw = new Dictionary<string, JToken>();

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_raw.ContainsKey("hMusic")) Qualities.Add(SongQuality.HQ);
            if (_raw.ContainsKey("mMusic")) Qualities.Add(SongQuality.MQ);
            if (_raw.ContainsKey("lMusic")) Qualities.Add(SongQuality.LQ);
        }
    }
}