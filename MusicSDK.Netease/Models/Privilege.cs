using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MusicSDK.Netease.Models
{

    public enum PriStatus
    {
        Normal,
        NoCopyright,
        Paid,
        Digital,
        VIP,
        DownloadVIP, // Need VIP to Downlaod
    }
    class Privilege
    {
        public long Id { get; set; }
        
        public int Maxbr { get; set; }

        public PriStatus Status { get; set; } = PriStatus.Normal;

        [JsonExtensionData]
        private Dictionary<string, JToken> raw = new Dictionary<string, JToken>();

        [OnDeserialized]
        private void onDeserialized(StreamingContext ctx)
        {
            var st = (int)raw["st"];
            var fee = (int)raw["fee"];
            var paid = (int)raw["payed"];

            if(st < 0) {
                Status = PriStatus.NoCopyright;
                return;
            }
            if (paid > 0)
            {
                Status = PriStatus.Paid;
                return;
            }

            if (fee == 1) {
                Status = PriStatus.VIP;
            } else if(fee == 4)
            {
                Status = PriStatus.Digital;
            } else if(fee == 8)
            {
                Status = PriStatus.DownloadVIP;
            }
        }

    }
}
