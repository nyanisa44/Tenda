using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class CommissionFileStatus
    {
        public int Id { get; set; }
        public string UserUid { get; set; }
        public string FileName { get; set; }
        public string Location { get; set; }
        public int Size { get; set; } //in bytes
        public int  ImportFileId {get;set;}
        public string Status { get; set; }
        public DateTime? ComissionRunDateFrom { get; set; }
        public DateTime? ComissionRunDateTo { get; set; }
        public int? SupplierId { get; internal set; }

        //Nav
        [ForeignKey("ImportFileId")]
        public ImportFile ImportFile { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }
    }
}