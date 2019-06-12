using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class BasicApplicationResponse
    {
        public int Id { get; set; }
        public int? ApplicationStatus_Id { get; set; }
        public int? ApplicationType_Id { get; set; }
        public int? Client_Id { get; set; }

        public int? Product_Id { get; set; }
        public string Product_Supplier_Name { get; set; }
        public int? SupplierId { get; set; }
        public string MemberNumber { get; set; }

        public bool? Deleted { get; set; }

    }
}