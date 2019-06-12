using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class AdvisorDocument
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public bool IsRequired { get; set; }
        public bool IsExpired { get; set; }
        public bool Deleted { get; set; }
        public bool Uploaded { get; set; }
        public string File { get; set; }
        public string OriginalFileName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ValidFromDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ValidToDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ModifiedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        public int DocumentTypeId { get; set; }

        public int? Advisor_Id { get; set; }
        //Nav Link
        [ForeignKey("Advisor_Id")]
        public Advisor Advisor { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}