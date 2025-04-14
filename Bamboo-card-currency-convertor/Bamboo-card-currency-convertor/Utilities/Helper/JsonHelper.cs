using System.Text.Json;

namespace Bamboo_card_currency_convertor.Utilities.Helper
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(string content)
        {
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }
    }
}
