using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TendaAdvisors.Controllers;

namespace TendaAdvisors.Models
{
    public class ImportFile
    {
        public int Id { get; set; }
        public string UserUid { get; set; }
        public string FileName { get; set; }
        public string Location { get; set; }
        public int Size { get; set; } //in bytes
        public string Data { get; set; } //Base64Data (temp instead of DTO) wasting column in DB!
        public byte[] BinaryData { get; set; } //Used for decoded Base64Data (temp instead of DTO) wasting column in DB!
        public DateTime CreatedDate { get; set; }
        public DateTime? DateImported { get; set; }
        public bool ImportSuccess { get; set; }

        public int ImportTypeId { get; set; }
        public int AdvisorId { get; set; }

        public FieldMap FieldMap { get; set; }

    }
}