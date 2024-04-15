using System;
using System.Collections.Generic;

namespace SubscriptionManagerApp.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Subscription> Subs { get; set; } = new List<Subscription>();
}
