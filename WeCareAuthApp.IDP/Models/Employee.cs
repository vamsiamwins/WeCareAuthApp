namespace WeCareAuthApp.IDP.Models
{
    public class Employee
    {
        //public int Id { get; set; } // New primary key
        public int EmployeeId { get; set; } // Existing field, no longer a primary key
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SSN { get; set; }
        public string PrimaryPhoneType { get; set; }
        public string PrimaryEmailType { get; set; }
        public bool OptInCommunication { get; set; }
    }
}
