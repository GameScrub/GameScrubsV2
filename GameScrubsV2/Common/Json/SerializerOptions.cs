using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameScrubsV2.Common.Json;

public static class SerializerOptions
{
	/// <summary>
	/// Default json serializer options.
	/// </summary>
	public readonly static JsonSerializerOptions DefaultJsonSerializerOptions = new(JsonSerializerDefaults.Web)
	{
		Converters =
		{
			new JsonStringEnumConverter(),
			new GuidConverter(),
		},
	};
}