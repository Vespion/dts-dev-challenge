using System.Text.Json;
using System.Text.Json.Serialization;

namespace DtsDevChallenge.Common;

/// <summary>
/// A <see cref="JsonSerializerContext"/> that is loaded with the source generated serializers for common types (such as <see cref="Task"/>)
/// </summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(TaskItem[]))]
public partial class CommonJsonContext : JsonSerializerContext;