using System.ComponentModel.DataAnnotations;

namespace WeCareAuthApp.IDP.Models
{
    public class Company
    {
        [Key] 
        public int Id { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPin { get; set; }
        public string CompanyName { get; set; }
    }
}
