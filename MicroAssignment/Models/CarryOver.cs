using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MicroAssignment.Models
{
    public class CarryOver
    {
        public int CarryOverId { get; set; }

        public int? CourseID { get; set; }
        public Course Course { get; set; }

        [Display(Name = "Continues Assessment:")]
        [Range(0, 30)]
        public decimal? TestScore { get; set; }

        [Display(Name = "Exam Score:")]
        [Range(0, 70)]
        public decimal? ExamScore { get; set; }

        [Display(Name = "Total Score:")]
        public decimal? TotalScore { get; set; }

        public string Grade { get; set; }

        public decimal? GradePoint { get; set; }

        public string Remark { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }

        public bool IsCarryover { get; set; }

        public int? UserId { get; set; }

        public DateTime? Date { get; set; }
    }
}