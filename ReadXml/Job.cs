using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ReadXml
{


    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("job",Namespace = "", IsNullable = false)]
    public class Job
    {

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("ompany")]
        public string Company { get; set; }

        [XmlElement("posted_date")]
        public string PostedDate { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        /// <remarks/>
        [XmlElement("clickcastid")]
        
        public jobClickcastid[] ClickCastId { get; set; }

        /// <remarks/>
        [XmlElement("responsibilities")]
        public string Responsibilities { get; set; }

        /// <remarks/>
        [XmlElement("company_id")]
        public uint CompanyId { get; set; }

        /// <remarks/>
        [XmlElement("job_id")]
        public string JobId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return ClickCastId.FirstOrDefault(f => f.Type == "child").Value; }
        }

        /// <remarks/>
        [XmlElement("city")]
        public string City { get; set; }

        /// <remarks/>
        [XmlElement("state")]
        public string State { get; set; }

        /// <remarks/>
        [XmlElement("zip")]
        public uint Zip { get; set; }

        /// <remarks/>
        [XmlElement("country")]
        public string Country { get; set; }

        /// <remarks/>
        [XmlElement("gradyear")]
        public string GradYear { get; set; }

        /// <remarks/>
        [XmlElement("jobtype")]
        public string JobType { get; set; }

        /// <remarks/>
        [XmlElement("applicationtype")]
        public string ApplicationType { get; set; }

        /// <remarks/>
        [XmlElement("comptype")]
        public string CompType { get; set; }

        /// <remarks/>
        [XmlElement("biztype")]
        public string BizType { get; set; }

        /// <remarks/>
        [XmlElement("locationtype")]
        public string LocationType { get; set; }

        /// <remarks/>
        [XmlElement("bidtype")]
        public string BidType { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class jobClickcastid
    {

        /// <remarks/>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <remarks/>
        [XmlText]
        public string Value { get; set; }
    }




}
