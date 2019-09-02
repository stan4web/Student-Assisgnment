using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroAssignment.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public UserProfile UserProfile { get; set; }
        public string PostContent { get; set; }
        public DateTime PostDate { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public string ShortPost
        {
            get
            {
                if (this.PostContent.Length > 200)
                    return this.PostContent.Substring(0, 200) + "...";
                else
                    return this.PostContent;
            }
        }

        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
        public bool? IsPublic { get; set; }
    }
}