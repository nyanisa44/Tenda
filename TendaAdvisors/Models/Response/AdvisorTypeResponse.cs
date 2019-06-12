using System.Collections.Generic;

namespace TendaAdvisors.Models.Response
{
    public class AdvisorTypeResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<DocumentTypeResponse> DocumentTypes { get; set; }
    }
}