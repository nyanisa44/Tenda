using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class Query
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string UserUID { get; set; }
        public string Note { get; set; }
        public bool  Deleted{ get; set; }


        //For dropdown in view
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<DateTime> QuerySent { get; set; }

        public int? QueryType_Id { get; set; }
        public int? Advisor_Id { get; set; }
        public int? Application_Id { get; set; }
        //Nav
        [ForeignKey("QueryType_Id")]
        public QueryType QueryType { get; set; }
        [ForeignKey("Advisor_Id")]
        public Advisor Advisor { get; set; }
        [ForeignKey("Application_Id")]
        public Application Application { get; set; }
        public string QueryName { get; internal set; }
    }
}