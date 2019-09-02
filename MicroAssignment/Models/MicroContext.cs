using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace MicroAssignment.Models
{
    public class MicroContext:DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<ReplyAssignment> ReplyAssignments { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<Material> Materials { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tutorial> Tutorials { get; set; }

        public DbSet<School> Schools { get; set; }
        public DbSet<Advertise> Advertising { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<CarryOver> CarryOvers { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<EnrolledCourse> EnrolledCourses { get; set; }
        public DbSet<Session> Sessions { get; set; }
        
    }
}