namespace SubscriptionManagerApp.Messages
{
    public class SubsApiHomeDTO
    {
        public Dictionary<string, Link>? Links { get; set; }

        public string? ApiVersion { get; set; }

        public string? Creator { get; set; }
    }
}
