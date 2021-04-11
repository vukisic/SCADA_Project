using Newtonsoft.Json;

namespace Core.Common.Json
{
    public static class JsonTool
    {
        public static string Serialize<T>(T item)
        {
            return JsonConvert.SerializeObject(item, Formatting.Indented);
        }

        public static T Deserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
