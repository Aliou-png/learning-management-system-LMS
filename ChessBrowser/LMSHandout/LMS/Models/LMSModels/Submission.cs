using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public string UId { get; set; } = null!;
        public uint? Score { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Submitted { get; set; }
        public uint AssignmentId { get; set; }

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual Student UIdNavigation { get; set; } = null!;
    }
}
