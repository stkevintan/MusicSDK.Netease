using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

/*
normal:
{
  "data": [
    {
      "id": 29307041,
      "url": "http://m8.music.126.net/20200629193836/6d8a31f3c5099230e9fd14cf03f2cef7/ymusic/1f2e/acda/5358/590cf6063232db1b861cff50de5b4bfe.mp3",
      "br": 128000,
      "size": 4296831,
      "md5": "590cf6063232db1b861cff50de5b4bfe",
      "code": 200,
      "expi": 1200,
      "type": "mp3",
      "gain": 0,
      "fee": 8,
      "uf": null,
      "payed": 0,
      "flag": 4,
      "canExtend": false,
      "freeTrialInfo": null,
      "level": "standard",
      "encodeType": "mp3"
    }
  ],
  "code": 200
}
*/
/*
vip:
{
  "data": [
    {
      "id": 26127499,
      "url": "http://m7.music.126.net/20200629194246/e67e83358ddad21825c61fd3c202a2ac/ymusic/ts/free/756163fd59bbf477ee81353b1b222a2c.mp3",
      "br": 128014,
      "size": 401284,
      "md5": "756163fd59bbf477ee81353b1b222a2c",
      "code": 200,
      "expi": 1200,
      "type": "mp3",
      "gain": 0,
      "fee": 1,
      "uf": null,
      "payed": 0,
      "flag": 1093,
      "canExtend": false,
      "freeTrialInfo": {
        "start": 72,
        "end": 97
      },
      "level": null,
      "encodeType": null
    }
  ],
  "code": 200
}
*/

#nullable disable
namespace MusicSDK.Netease.Models
{
    public enum SongQuality
    {
        SQ = 999000,
        HQ = 320000,
        MQ = 192000,
        LQ = 128000
    }


    public class FreeTrialInfo
    {
        public int Start { get; set; }

        public int End { get; set; }
    }

    public class SongUrl
    {
        public long Id { get; set; }

        public int Br { get; set; }

        public string Url { get; set; }

        // mp3, ...
        public string EncodeType { get; set; }

        public string Level { get; set; } = "standard";

        public string Md5 { get; set; }

        public int Size { get; set; }

        public FreeTrialInfo FreeTrialInfo { get; set; }

        public SongQuality Quality { get; set; } = SongQuality.LQ;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (Br >= (int)SongQuality.SQ) Quality = SongQuality.SQ;
            else if (Br >= (int)SongQuality.HQ) Quality = SongQuality.HQ;
            else if (Br >= (int)SongQuality.MQ) Quality = SongQuality.MQ;
            else Quality = SongQuality.LQ;
        }

    }
}