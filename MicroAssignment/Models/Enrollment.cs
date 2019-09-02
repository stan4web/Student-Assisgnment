using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroAssignment.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public int UserId { get; set; }
        public UserProfile UserProfile { get; set; }

        public int SessionId { get; set; }
        public Session Session { get; set; }

        public bool IsSaved { get; set; }
        public decimal? GPA { get; set; }
        public decimal? CGPA { get; set; }

        public bool? IsApproved { get; set; }
        public bool? IsSubmitted { get; set; }

        public ICollection<EnrolledCourse> EnrolledCourses { get; set; }
        public ICollection<CarryOver> CarryOvers { get; set; }
    }
}