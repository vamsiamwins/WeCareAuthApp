namespace WeCareAuthApp.IDP
{
    public class RegistrationDTO
    {
        public string SessionKey { get; set; }
        //public int EmployeeId { get; set; }
        public string SSN { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneType { get; set; } // Dropdown: Personal, Office, Home
        public string EmailType { get; set; } // Dropdown: Primary, Secondary, Other
        public bool OptInCommunication { get; set; }
    }
}
