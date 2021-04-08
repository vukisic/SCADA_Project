namespace Core.Common.PubSub
{
    public class Subscription
    {
        public string ConnectionString { get; set; }
        public string Topic { get; set; }
        public string SubscriptionName { get; set; }

        public Subscription()
        {
            ConnectionString = "Endpoint=sb://projekat.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=wJAF6hCmIfG81wU0AZVDdcUAv35T0VrEh1OaPzJhn+Y=";
            Topic = "mainTopic";
            SubscriptionName = "guisub";
        }
    }
}
