using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class ApplicationDocument
    {
        public int Id { get; set; }
        //[MaxLength(50)]
        public string Title { get; set; }
        //[MaxLength(50)]
        public string Location { get; set; }
        public string File { get; set; }
        //[MaxLength(50)]
        public string OriginalFileName { get; set; }
        public bool IsRequired { get; set; }
        public bool Uploaded { get; set; }
        public bool IsExpired { get; set; }
        public bool? Deleted { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ValidFromDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ValidToDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ModifiedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }
        //FK
        public int ProductId { get; set; }
        public int ApplicationId { get; set; }
        public int DocumentTypeId { get; set; }
        //Nav
        public Application Application { get; set; }
        public Product Product { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}