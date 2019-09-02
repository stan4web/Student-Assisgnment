using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroAssignment.Models
{
    public class Tutorial
    {
        public int TutorialId { get; set; }
        public string CourseName { get; set; }
        public string Topic { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public bool IsHidden { get; set; }
        public int UserId { get; set; }
        public UserProfile UserProfile { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }


        public string ShortTutor
        {
            get
            {
                if (this.Content.Length > 1000)
                    return this.Content.Substring(0, 1000) + "...";
                else
                    return this.Content;
            }
        }

    }
}