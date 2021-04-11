namespace Core.Common.PubSub
{
    public static class PubSubMessageSerializer
    {
        public static string Serialize(PubSubMessage message)
        {
            return Json.JsonTool.Serialize<PubSubMessage>(message);
        }

        public static T Desetialize<T>(string content)
        {
            return Json.JsonTool.Deserialize<T>(content);
        }
    }
}
