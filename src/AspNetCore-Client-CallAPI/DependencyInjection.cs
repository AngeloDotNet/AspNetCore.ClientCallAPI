using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspNetCore_Client_CallAPI;

public static class DependencyInjection
{
    // Reuse JsonSerializerOptions to avoid repeated allocations
    public static readonly JsonSerializerOptions jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
}