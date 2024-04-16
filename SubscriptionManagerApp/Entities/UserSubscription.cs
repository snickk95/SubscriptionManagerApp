namespace SubscriptionManagerApp.Entities
{
    public class UserSubscription
    {
        public int SubscriptionId { get; set; }

        public int UserId { get; set; }

        public Subscription? Subscription { get; set; }

        public User? User { get; set; }
    }
}
