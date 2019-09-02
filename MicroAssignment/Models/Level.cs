using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MicroAssignment.Models
{
    public class Level
    {
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public string Program { get; set; }
        public string FullLevel { get { return LevelName + " - " + Program; } }

        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}