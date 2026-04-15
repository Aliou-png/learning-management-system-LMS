using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategories = new HashSet<AssignmentCategory>();
            Enrolleds = new HashSet<Enrolled>();
        }

        public uint ClassId { get; set; }
        public string SemesterSeason { get; set; } = null!;
        public uint SemesterYear { get; set; }
        public TimeOnly? Start { get; set; }
        public TimeOnly? End { get; set; }
        public string? Location { get; set; }
        public string Professor { get; set; } = null!;
        public uint CourseId { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual Professor ProfessorNavigation { get; set; } = null!;
        public virtual ICollection<AssignmentCategory> AssignmentCategories { get; set; }
        public virtual ICollection<Enrolled> Enrolleds { get; set; }
    }
}
