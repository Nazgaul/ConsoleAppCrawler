﻿using System;
using Newtonsoft.Json;

namespace ConsoleAppCrawler
{
    //[SerializePropertyNamesAsCamelCase]
    //[DocumentDbModel("Crawl-Url")]
    public class CrawlModel
    {
        protected CrawlModel()
        {
        }

        public CrawlModel(string url, string domain, string name, string content, string metaDescription,string image, string id)
        {
            Url = url;
            Name = name;
            Content = content;       
            MetaDescription = metaDescription;
            Domain = domain;
            Id = id;
            Image = image;
            Type = ItemType.Flashcard;
        }
        public CrawlModel(string url, string name, string content, string university,
            string course, string[] tags, DateTime? urlDate, int? views,
            string metaDescription, string image, string[] metaKeywords, string domain, string id,
            ItemType type = ItemType.Undefined)
        {
            Url = url;
            Name = name;
            Content = content;
            University = university;
            Course = course;
            Tags = tags;
            UrlDate = urlDate;
            Views = views;
            MetaDescription = metaDescription;
            Image = image;
            MetaKeywords = metaKeywords;
            Domain = domain;
            Id = id;
            Type = type;
        }

        public string Url { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }


        public string University { get; set; }

        public string Course { get; set; }


        public string[] Tags { get; set; }

        public DateTime? UrlDate { get; set; }

        public int? Views { get; set; }

        public string MetaDescription { get; set; }

        public string Image { get; set; }

        public string[] MetaKeywords { get; set; }

        public string Domain { get; set; }

        public ItemType Type { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }


        public override string ToString()
        {
            return Id;
        }
    }

    public enum ItemType
    {
        Undefined = 0,
        Flashcard = 1,
        Quiz = 2,
        Document, // not used

        Link // not used
        //Homework = 5,
        //StudyGuide = 7,
        //Exam=8,
        //ClassNote=9,
        //Syllabus=10
    }
}