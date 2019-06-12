using System;

namespace TendaAdvisors.Models.Response
{
    public class AdvisorDocumentResponse
    {
        public int Id { get; set; }
        public int? DocumentType_Id { get; set; }
        public string DocumentType { get; set; }
        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        public bool? Deleted { get; set; }
        public string Title { get; set; }
    }
}