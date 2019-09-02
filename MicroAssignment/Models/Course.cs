using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroAssignment.Models
{
    public class Course
    {
        public int CourseID { get; set; }

        public string CourseName { get; set; }

        public int DepartmentID { get; set; }
        public Department Department { get; set; }

        public int CreditUnit { get; set; }
        public string CourseCode { get; set; }


        public int? SessionId { get; set; }
        public Session Session { get; set; }
    }
}