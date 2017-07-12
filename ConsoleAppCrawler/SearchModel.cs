using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using ProtoBuf;

namespace ConsoleAppCrawler
{
    [SerializePropertyNamesAsCamelCase]
    [ProtoContract]
    public class SearchModel
    {
        [Key]
        [ProtoMember(1)]
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
    }
}
