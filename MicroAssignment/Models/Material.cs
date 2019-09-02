using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroAssignment.Models
{
    public class Material
    {
        public int MaterialId { get; set; }
        public string CategoryName { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public string Topic { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public string DocumentUrl { get; set; }
        public DateTime DateUploaded { get; set; }
        public int UserId { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}