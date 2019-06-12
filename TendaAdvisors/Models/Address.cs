using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class Address
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string Street1 { get; set; }
        [MaxLength(50)]
        public string Street2 { get; set; }
        [MaxLength(50)]
        public string Street3 { get; set; }
        [MaxLength(30)]
        public string Suburb { get; set; }
        [MaxLength(50)]
        public string PostalCode { get; set; }
        [MaxLength(30)]
        public string City { get; set; }
        public string MapUrl { get; set; }
        public bool? Deleted { get; set; }
        public int? Contact_Id { get; set; }
        public int? AddressType_Id { get; set; }
        public int? Province_Id { get; set; }
        public int? Country_Id { get; set; }
        //Nav Link
        [ForeignKey("Contact_Id")]
        public Contact Contact { get; set; }
        [ForeignKey("AddressType_Id")]
        public AddressType AddressType { get; set; }
        [ForeignKey("Province_Id")]
        public Province Province { get; set; }
        [ForeignKey("Country_Id")]
        public Country Country { get; set; }
        
    }
}