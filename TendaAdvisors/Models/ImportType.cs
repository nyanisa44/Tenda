using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class ImportType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string ActionUrl { get; set; }
        public string SupportedFileTypes { get; set; }
        public bool? Enabled { get; set; }
    }
}