namespace Core.Common.ServiceBus.Dtos
{
    public interface IIdentifiedObject
    {
        string Description { get; set; }
        long GID { get; set; }
        string MRID { get; set; }
        string Name { get; set; }
    }
}
