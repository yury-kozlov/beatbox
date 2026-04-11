using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Beater;

public static class Serialization
{
    public static string ToJson(this SequenceDesign seq)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new TypedBaseClassesContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // circular reference: Sequence -> SequenceStart -> Sequence -> ...
        };
        return JsonConvert.SerializeObject(seq, Formatting.Indented, settings);
    }

    public static T? FromJson<T>(this string json)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Converters = { new SoundConverter() },
        };
        return JsonConvert.DeserializeObject<T>(json, settings);
    }
}

public class SoundConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(Sound);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);

        var typeToken = obj["$type"]?.Value<string>();
        if (typeToken != null)
        {
            var actualType = Type.GetType(typeToken);
            if (actualType != null && actualType != typeof(Sound))
            {
                return serializer.Deserialize(obj.CreateReader(), actualType);
            }
        }

        var name = obj["Name"]?.Value<string>() ?? string.Empty;
        var sound = new Sound(name);
        serializer.Populate(obj.CreateReader(), sound);
        return sound;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        => throw new NotSupportedException($"{nameof(SoundConverter)} does not support writing.");
}

/// <summary>
/// Adds information about concrete type to JSON only if current property is a base class (or an interface).
/// All other classes, including system types are serialized without type info.
/// </summary>
public class TypedBaseClassesContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);
        if (member is PropertyInfo propertyInfo && !propertyInfo.CanWrite)
        {
            // ignore calculated properties during serialization
            property.Ignored = true;
        }
        return property;
    }

    protected override JsonContract CreateContract(Type objectType)
    {
        JsonContract contract = base.CreateContract(objectType);

        if (contract is JsonObjectContract obj)
        {
            foreach (var p in obj.Properties)
            {
                if (IsSystem(p.PropertyType))
                {
                    continue;
                }
                if (IsBase(p.PropertyType) || IsSound(p.PropertyType))
                {
                    // add "$type" field only for custom types with derived classes:
                    p.TypeNameHandling = TypeNameHandling.All;
                }
            }
        }

        if (contract is JsonContainerContract list)
        {
            if (!IsSystem(objectType) && (IsBase(objectType) || IsListOfSounds(objectType)))
            {
                // add "$type" field only for custom types with derived classes:
                list.ItemTypeNameHandling = TypeNameHandling.All;
            }
        }

        return contract;
    }

    private bool IsSystem(Type? t) => (t?.Namespace ?? "").StartsWith("System");

    private bool IsBase(Type? type)
    {
        if (type is null)
        {
            return false;
        }
        if (type.IsAbstract || type.IsInterface)
        {
            return true;
        }
        if (type == typeof(Sound) || type.IsSubclassOf(typeof(Sound)))
        {
            return true;
        }
        return Assembly.GetAssembly(type)?.GetTypes().Any(t => t.BaseType == type) ?? false;
    }

    private bool IsSound(Type? type)
    {
        if (type is null)
        {
            return false;
        }
        return type == typeof(Sound) || type.IsSubclassOf(typeof(Sound));
    }

    /// <summary>
    /// Checks if type is derived from List<Sound>
    /// </summary>
    private bool IsListOfSounds(Type? type)
    {
        var t = type;
        while (t != null)
        {
            if (t.IsGenericType &&
                t.GetGenericTypeDefinition() == typeof(List<>) &&
                typeof(Sound).IsAssignableFrom(t.GetGenericArguments()[0]))
            {
                return true;
            }
            t = t.BaseType;
        }
        return false;
    }
}