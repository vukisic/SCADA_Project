namespace Core.Common.ServiceBus.Commands
{
    /**
     * Example Command class (feel free to delete it)
     * 
     * NOTE: Take care when using auto-complete, ICommand in this case is from NServiceBus, and NOT System.Windows.Input (don't mix these two)
     */
    public class DemoCommand : NServiceBus.ICommand
    {
        /* 
         * Add any properties you want just like you would in any other DTO class (ex. WCF, ASP.NET...)
         */
        public string DemoProperty { get; set; }
    }
}
