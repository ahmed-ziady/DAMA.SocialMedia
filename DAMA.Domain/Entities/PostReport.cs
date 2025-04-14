using System;
using System.Collections.Generic;

namespace DAMA.Domain.Entities;

public partial class PostReport
{
    public int ReportId { get; set; }

    public int PostId { get; set; }

    public int ReportTypeId { get; set; }

    public int Id { get; set; }

    public string? ReportStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual ReportType ReportType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
