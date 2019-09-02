using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroAssignment.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public string FullDepartment { get { return DepartmentName + " [" + DepartmentCode+"]"; } }

        public int SchoolId { get; set; }
        public School School { get; set; }


        public string SchoolCode { get; set; }

        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<Material> Materials { get; set; }




        public string DepartmentInfo { get { return DepartmentName + "(  " + SchoolCode + "  )"; } }
    }
}