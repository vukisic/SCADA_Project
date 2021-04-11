using System.Runtime.Serialization;

namespace Core.Common.PubSub
{
    [DataContract]
    public enum ContentType
    {
        [EnumMember]
        NMS_UPDATE,

        [EnumMember]
        SCADA_UPDATE,

        [EnumMember]
        SCADA_HISTORY,

        [EnumMember]
        SCADA_DOM,

        [EnumMember]
        SCADA_HISTORY_GRAPH,

        [EnumMember]
        CE_UPDATE,

        [EnumMember]
        CE_HISTORY_GRAPH,
    }

    [DataContract]
    public enum Sender
    {
        [EnumMember]
        NMS,

        [EnumMember]
        SCADA,

        [EnumMember]
        CE,

        [EnumMember]
        GUI
    }

    [DataContract]
    public class PubSubMessage
    {
        [DataMember]
        public Sender Sender { get; set; }

        [DataMember]
        public ContentType ContentType { get; set; }

        [DataMember]
        public string Content { get; set; }
    }
}
