using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViViPOSLive_Sample_AspNet.Models
{
    public class Product
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}