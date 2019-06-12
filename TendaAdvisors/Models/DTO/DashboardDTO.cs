using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TendaAdvisors.Models.DTO
{
    public class ApplicationStatusDTO
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int Count { get; set; }
    }


    public class AdvisorShareUnderSupervisionDTO
    {
        public int AdvisorId { get; set; }
        public int LicenseTypeId { get; set; }
        public string supplier { get; set; }
        public string product { get; set; }
        public double Share { get; set; }
        public bool underSupervision { get; set; }
        public int Advisor { get; set; }
        [DataType(DataType.Date)]
        public DateTime? validCommissionFromDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? validCommissionToDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ValidFromDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ValidToDate { get; set; }
    }

    public class DashboardDTO
    {
        public Advisor Advisor { get; set; }
        public List<Application> MyApplications { get; set; }
        public List<ApplicationStatusDTO> ApplicationStatuses { get; set; }
        public List<AdvisorShareUnderSupervisionDTO> AdvisorShareUnderSupervisions { get; set; }
        

        public bool Role { get; set; }
        public bool advisorRole { get; set; }
        public int ID { get; set; }
    }
}