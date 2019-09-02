  using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroAssignment.Models
{
    public class School
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string SchoolCode { get; set; }
        public bool? IsUniversity { get; set; }

        public virtual ICollection<Department> Departments { get; set; }
    }
}