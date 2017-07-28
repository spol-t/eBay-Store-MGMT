using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMGMT
{
    public class Person
    {
        private string email;
        private string firstName;
        private string lastNamel;
        private string address1;
        private string address2;
        private string city;
        private string state_province;
        private string zip_postal;
        private string country;
        private string phone;
        internal object lastName;

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State_province { get; set; }
        public string Zip_postal { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
    }
}
