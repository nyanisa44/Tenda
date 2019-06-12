namespace TendaAdvisors.Models.Response
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public LicenseTypeResponse LicenseType { get; set; }
    }
}