using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string ApplicationNumber { get; set; }
        public string AdvisorCode { get; set; }
        public bool? Deleted { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ModifiedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }
 
        // Foreign Keys
        public int? Client_Id { get; set; }
        public int? Advisor_Id { get; set; }
        public int? ApplicationStatus_Id { get; set; }
        public int? ApplicationType_Id { get; set; }
        public int? Product_Id { get; set; }
        //Nav
        [ForeignKey("Advisor_Id")]
        public Advisor Advisor { get; set; }
        [ForeignKey("Client_Id")]
        public Contact Client { get; set; }
        [ForeignKey("ApplicationStatus_Id")]
        public ApplicationStatus ApplicationStatus { get; set; }
        [ForeignKey("ApplicationType_Id")]
        public ApplicationType ApplicationType { get; set; }
        [ForeignKey("Product_Id")]
        public Product Product { get; set; }
        
        public ICollection<ApplicationDocument> ApplicationDocuments { get; set; }
        
        public ICollection<Query> Queries { get; set; }
        
        public virtual ICollection<ApplicationSupplier> ApplicationSuppliers { get; set; }
    }
}