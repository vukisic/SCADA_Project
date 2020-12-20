using NServiceBus;

namespace Core.Common.ServiceBus.Events
{
    /**
     * Example event class (feel free to delete it)
     * **/
    public class DemoEvent : IEvent
    {
        /* 
         * Add any properties you want just like you would in any other DTO class (ex. WCF, ASP.NET...)
         */
        public string DemoProperty { get; set; }
    }
}
