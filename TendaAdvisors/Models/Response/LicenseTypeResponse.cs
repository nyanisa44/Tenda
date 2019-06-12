namespace TendaAdvisors.Models.Response
{
    public class LicenseTypeResponse
    {
        public int Id { get; set; }
        public int? Supplier_Id  { get; set; }
        public string SupplierName { get; set; }
        public string ProductName { get; set; }
        public int LicenseCategoryId { get; set; }
        public string SubCategory { get; set; }
        public string Description { get; set; }
    }
}