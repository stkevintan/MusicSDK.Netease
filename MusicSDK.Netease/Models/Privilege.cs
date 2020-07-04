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
    public class Privilege
    {
        public long Id { get; set; }

        public int Maxbr { get; set; }

        public PriStatus Status { get; set; } = PriStatus.Normal;

        [JsonExtensionData]
        private readonly Dictionary<string, JToken> raw = new Dictionary<string, JToken>();

        public Privilege() { }

        public Privilege(int st, int fee, int paid)
        {
            Init(st, fee, paid);
        }

        private void Init(int st, int fee, int paid)
        {
            if (st < 0)
            {
                Status = PriStatus.NoCopyright;
                return;
            }
            if (paid > 0)
            {
                Status = PriStatus.Paid;
                return;
            }

            if (fee == 1)
            {
                Status = PriStatus.VIP;
            }
            else if (fee == 4)
            {
                Status = PriStatus.Digital;
            }
            else if (fee == 8)
            {
                Status = PriStatus.DownloadVIP;
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            var st = (int)raw["st"];
            var fee = (int)raw["fee"];
            var paid = (int)raw["payed"];
            Init(st, fee, paid);
        }

    }
}
