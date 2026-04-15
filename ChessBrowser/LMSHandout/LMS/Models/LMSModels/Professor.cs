using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professor
    {
        public Professor()
        {
            Classes = new HashSet<Class>();
        }

        public string UId { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? Dob { get; set; }

        public virtual Department SubjectNavigation { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; }
    }
}
