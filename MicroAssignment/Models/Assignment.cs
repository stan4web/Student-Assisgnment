using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroAssignment.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string Topic { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public int? SchoolId { get; set; }
        public School School { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public int LevelId { get; set; }
        public Level Levels { get; set; }

        [Display(Name = "Lecturers Name")]
        public int UserId { get; set; }
        public UserProfile UserProfile { get; set; }

        public DateTime? Date { get; set; }
        public DateTime? ClosingDate { get; set; }

        public string FilePath { get; set; }
    }
}