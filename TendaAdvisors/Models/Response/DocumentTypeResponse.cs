using System;

namespace TendaAdvisors.Models.Response
{
    public class DocumentTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        public string DocumentTypeName { get; set; }
        public int? DocumentTypeId { get; set; }
        public bool? Uploaded { get; set; }
        public bool? IsExpired { get; set; }
        public int? ApplicationId { get; set; }
        public BasicContactResponse client { get; set; }
    }
}