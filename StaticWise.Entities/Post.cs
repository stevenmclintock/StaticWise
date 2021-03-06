﻿using Newtonsoft.Json;
using System;

namespace StaticWise.Entities
{
    public struct Post
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("friendlyUrl")]
        public string FriendlyUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonIgnore]
        public string FileContent { get; set; }

        [JsonIgnore]
        public string FilePath { get; set; }

        [JsonIgnore]
        public DateTime FileDate { get; set; }
    }
}