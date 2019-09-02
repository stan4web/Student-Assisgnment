using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroAssignment.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string Content { get; set; }
        public DateTime CommentDate { get; set; }
        public int UserId { get; set; }

        public int? TutorialId { get; set; }

    }
}