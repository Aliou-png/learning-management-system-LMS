using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCategory
    {
        public AssignmentCategory()
        {
            Assignments = new HashSet<Assignment>();
        }

        public uint CategoryId { get; set; }
        public string Category { get; set; } = null!;
        public byte? Weight { get; set; }
        public uint ClassId { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
