using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using ProtoBuf;

namespace ConsoleAppCrawler
{
    [SerializePropertyNamesAsCamelCase]
    [ProtoContract]
    public class SearchModel
    {
        [Key]
        [ProtoMember(1)]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [ProtoMember(2)]
        [IsSearchable]
        public string Title { get; set; }

        [ProtoMember(3)]
        [IsSearchable]
        public string Content { get; set; }

        [ProtoMember(4)]
        [IsSearchable]
        public string MetaTitle { get; set; }

        [ProtoMember(5)]
        [IsSearchable]
        public string MetaDescription { get; set; }

        [ProtoMember(6)]
        [IsSearchable]
        public string[] MetaKeyword { get; set; }

        [ProtoMember(7)]
        [IsSearchable]
        public string[] Tags { get; set; }

        [ProtoMember(8)]
        public string Url { get; set; }


        [ProtoMember(9)]
        public string Image { get; set; }

        [ProtoMember(10)]
        public string University { get; set; }

        [ProtoMember(11)]
        public string Course { get; set; }

        [ProtoMember(12)]
        public string Views { get; set; }

        [ProtoMember(13)]
        public DateTime Date { get; set; }


        [ProtoMember(14)]
        public int ContentCount { get; set; }
    }
}