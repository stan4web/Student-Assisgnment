using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroAssignment.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public int UserId { get; set; }
        public UserProfile userProfile { get; set; }
        public string Password { get; set; }
    }
}