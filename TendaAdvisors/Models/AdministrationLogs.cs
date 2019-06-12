using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class AdministrstionLogs
    {
        public int Id { get; set; }
        public string Advisor_Name { get; set; }
        public TimestampAttribute Timestamp { get; set; }
        public string Details_JSON { get; set; }
        public string Action_Type { get; set; }
    }
}