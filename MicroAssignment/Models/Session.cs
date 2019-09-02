using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroAssignment.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public string SessionYear { get; set; }
        public string Semester { get; set; }

        public bool IsCurrent { get; set; }

        public string FullSession { get { return SessionYear + "/" + Semester + " [ " + IsCurrent+" ]"; } }
    }
}