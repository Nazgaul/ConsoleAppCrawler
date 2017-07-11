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
        public string Meta { get; set; }

        [ProtoMember(5)]
        public string Url { get; set; }
    }
}
