using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiddyDesktop.Models
{
    public class EmployeeDTO
    {
       
        public string username { get; set; }

        public string password { get; set; }

        public string dob { get; set; }

        public string gender { get; set; }

        public string role { get; set; }

        public bool? isActived { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }

        public byte[] image { get; set; }
    }
}