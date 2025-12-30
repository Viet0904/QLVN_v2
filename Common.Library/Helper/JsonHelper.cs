using Newtonsoft.Json;

namespace Common.Library.Helper
{
    public class JsonHelper
    {
        public static string SerializeObject<T>(T data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            });
        }

        public static T DeserializeObject<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            });
        }
    }
}
