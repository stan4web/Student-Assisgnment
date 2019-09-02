using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MicroAssignment.Models
{
    public class ReplyAssignment
    {
        public int ReplyAssignmentId { get; set; }
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public int LevelId { get; set; }
        public Level Levels { get; set; }

        [Display(Name="Lecturer Name")]
        public int UserId { get; set; }
        public UserProfile UserProfile { get; set; }

        public DateTime? Date { get; set; }
        public int StudentId { get; set; }
        public int? Score { get; set; }
        public int? TestScore { get; set; }
        public int? ExamScore { get; set; }
        public int? TotalSCore { get; set; }
        public string Avrage { get; set; }

        public string ShortReply
        {
            get
            {
                if (this.Content.Length > 200)
                    return this.Content.Substring(0, 200) + "...";
                else
                    return this.Content;
            }
        }

        public string FilePath { get; set; }
    }
}