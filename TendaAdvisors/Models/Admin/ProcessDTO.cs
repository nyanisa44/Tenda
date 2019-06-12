using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Admin
{
    public class ProcessDTO
    {
        public List<ImportFile> ImportFiles { get; set; }
        public int MyProperty { get; set; }
    }
}