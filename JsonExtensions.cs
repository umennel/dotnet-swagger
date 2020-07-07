using System.Text.Json;

namespace TodoList
{
    public static class JsonExtensions
    {
        public static bool TryGetBoolean(this JsonElement element, out bool result)
        {
            var (success, value) = element.ValueKind switch
            {
                JsonValueKind.True => (true, true),
                JsonValueKind.False => (true, false),
                JsonValueKind.String => (bool.TryParse(element.ToString(), out var v), v),
                _ => (false, default)
            };

            result = value;
            return success;
        }
    }
}
