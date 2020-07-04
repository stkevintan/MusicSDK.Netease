using MusicSDK.Netease.Library;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


#nullable disable
namespace MusicSDK.Netease.Models
{
    [EntityInfo(1000)]
    public class Playlist : IBaseModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int BookCount { get; set; }

        public string CoverImgUrl { get; set; }

        public User Creator { get; set; }

        public string Description { get; set; }

        public bool HighQuality { get; set; }

        public int PlayCount { get; set; }

        public bool Subscribed { get; set; }

        public int TrackCount { get; set; }

        public long UserId { get; set; }
    }

    public class PlaylistDetail : Playlist
    {
        public string BackgroundCoverUrl { get; set; }
        public int CommentCount { get; set; }

        [JsonConverter(typeof(DateConverter))]
        public DateTime CreateTime { get; set; }

        [JsonConverter(typeof(DateConverter))]
        public DateTime UpdateTime { get; set; }

        public int Privacy { get; set; }

        /// <summary>
        /// Song details, limit by parameters
        /// </summary>
        public List<Song> Tracks { get; set; }

        /// <summary>
        /// full song id list, no-limit
        /// </summary>
        [JsonIgnore]
        public List<long> TrackIds { get; set; }

        [JsonExtensionData]
        private readonly Dictionary<string, JToken> _raw = new Dictionary<string, JToken>();

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            if (_raw.TryGetValue("trackIds", out var token)) {
                TrackIds = token.Children().Select(x => (long)x["id"]).ToList();
            }
        }
    }
}