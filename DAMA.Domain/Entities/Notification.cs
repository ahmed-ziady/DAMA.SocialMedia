using System;
using System.Collections.Generic;

namespace DAMA.Domain.Entities;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int Id { get; set; }

    public int NotificationTypeId { get; set; }

    public string? Content { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual NotificationType NotificationType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
