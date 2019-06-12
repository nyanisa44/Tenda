using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TendaAdvisors.Models
{
    public class ApplicationStatus
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string Status { get; set; }
    }
}