using System;
using System.Collections.Generic;

namespace SubscriptionManagerApp.Entities;

public partial class Subscription
{
    public int SubscriptionId { get; set; }

    public string? ServiceName { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
