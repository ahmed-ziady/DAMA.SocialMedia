using System;
using System.Collections.Generic;

namespace DAMA.Domain.Entities;

public partial class ReportType
{
    public int ReportTypeId { get; set; }

    public string ReportTypeName { get; set; } = null!;

    public virtual ICollection<PostReport> PostReports { get; set; } = new List<PostReport>();
}
