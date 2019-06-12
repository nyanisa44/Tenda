using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace TendaAdvisors.Models
{
    public class AddressType
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string Name { get; set; } //Home, Office, Other

        //nav
        
        public ICollection<Address> Addresses { get; set; }
    }
}