using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Concurrent;
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
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // circular reference: Followers -> SequenceStart -> Followers -> ...
            Converters = { new FollowersConverter() },
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
    public override bool CanConvert(Type objectType) => objectType == typeof(SoundDesign);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);

        var typeToken = obj["$type"]?.Value<string>();
        if (typeToken != null)
        {
            var actualType = Type.GetType(typeToken);
            if (actualType != null && actualType != typeof(SoundDesign))
            {
                return serializer.Deserialize(obj.CreateReader(), actualType);
            }
        }

        var name = obj["Name"]?.Value<string>() ?? string.Empty;
        var sound = new SoundDesign(name);
        serializer.Populate(obj.CreateReader(), sound);
        return sound;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        => throw new NotSupportedException($"{nameof(SoundConverter)} does not support writing.");
}

public class FollowersConverter : JsonConverter<FollowersDesign>
{
    public override void WriteJson(JsonWriter writer, FollowersDesign? value, JsonSerializer serializer)
    {
        // By taking over Followers serialization, we bypass the ItemTypeNameHandling = TypeNameHandling.All
        // that TypedBaseClassesContractResolver sets on the Followers array contract.
        // Without it, concrete SoundDesign subtypes (Metronome, Kick, etc.) lose their $type field and
        // deserialize back as plain SoundDesign. We temporarily set TypeNameHandling.All on the shared
        // serializer so each item emits $type, then restore the original value afterward.
        var prevTypeNameHandling = serializer.TypeNameHandling;
        serializer.TypeNameHandling = TypeNameHandling.All;

        writer.WriteStartArray();
        if (value != null)
        {
            foreach (var item in value)
            {
                if (item is SequenceEnd)
                    continue;
                // typeof(SoundDesign) as the nominal type makes the declared/runtime type mismatch explicit,
                // ensuring $type is written even when TypeNameHandling is Auto instead of All.
                serializer.Serialize(writer, item, typeof(SoundDesign));
            }
        }
        writer.WriteEndArray();

        serializer.TypeNameHandling = prevTypeNameHandling;
    }

    public override FollowersDesign ReadJson(JsonReader reader, Type objectType, FollowersDesign? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var items = serializer.Deserialize<List<SoundDesign>>(reader) ?? [];
        var result = new FollowersDesign();
        foreach (var item in items)
        {
            result.Add(item);
        }
        return result;
    }
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
        if (member.DeclaringType == typeof(SequenceDesign) && member.Name == nameof(SequenceDesign.Strategy))
        {
            // Strategy is a proxy for Leader.Strategy and is already serialized as part of SequenceStart
            property.Ignored = true;
        }

        // Skip empty Followers: SoundDesign ctor initializes it to new FollowersDesign(), which is a non-null
        // reference and therefore not caught by DefaultValueHandling.Ignore.
        if (member.Name == "Followers" && member.DeclaringType == typeof(SoundDesign))
        {
            var vp = property.ValueProvider;
            property.ShouldSerialize = obj => (vp?.GetValue(obj) as ICollection)?.Count > 0;
        }

        // Skip default Strategy on SoundDesign: the field initializer is new FollowLeaderStrategy() with all
        // defaults, so if nothing was customized there is no need to round-trip it through JSON —
        // deserialization will reconstruct the same value from the field initializer.
        if (member.Name == nameof(SoundDesign.Strategy) && member.DeclaringType == typeof(SoundDesign))
        {
            var vp = property.ValueProvider;
            property.ShouldSerialize = obj => !IsDefaultStrategy(vp?.GetValue(obj) as AbstractStrategy);
        }

        // DefaultValueHandling.Ignore only skips CLR defaults (0, false, null).
        // For fields with non-CLR initializers (e.g. PlayEveryX=1, TrimIfExceedsParentLoop=true),
        // we construct a default instance of the declaring type once and compare member values against it.
        var initDefault = GetInitializerDefault(member);
        if (initDefault != null)
        {
            var vp = property.ValueProvider;
            property.ShouldSerialize = obj => !Equals(vp?.GetValue(obj), initDefault);
        }

        return property;
    }

    private static bool IsDefaultStrategy(AbstractStrategy? strategy)
        => strategy is FollowLeaderStrategy
        {
            DelayAfterLeader: 0,
            PlayEveryX: 1,
            FireAndForget: false,
            PlayEveryXOutOf: null,
            SilenceEveryXOutOf: null,
        };

    private static readonly ConcurrentDictionary<Type, object?> _defaultInstances = new();

    private static object? GetInitializerDefault(MemberInfo member)
    {
        var declaringType = member.DeclaringType;
        if (declaringType == null || declaringType.Namespace?.StartsWith("Beater") != true)
            return null;

        var instance = _defaultInstances.GetOrAdd(declaringType, t =>
        {
            if (!t.IsAbstract)
                try { return Activator.CreateInstance(t); } catch { return null; }

            // For abstract types, use any concrete subclass to read shared field initializer values.
            return Assembly.GetAssembly(t)?.GetTypes()
                .Where(sub => sub.IsSubclassOf(t) && !sub.IsAbstract)
                .Select(sub => { try { return Activator.CreateInstance(sub); } catch { return null; } })
                .FirstOrDefault(i => i != null);
        });

        if (instance == null) return null;

        var value = member switch
        {
            FieldInfo fi => fi.GetValue(instance),
            PropertyInfo pi => pi.CanRead ? pi.GetValue(instance) : null,
            _ => null,
        };

        if (value == null) return null;

        // Skip values that are also the CLR default — DefaultValueHandling.Ignore already handles those.
        var memberType = member is FieldInfo f ? f.FieldType : (member as PropertyInfo)?.PropertyType;
        if (memberType?.IsValueType == true && Equals(value, Activator.CreateInstance(memberType)))
            return null;

        return value;
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
        if (type == typeof(SoundDesign) || type.IsSubclassOf(typeof(SoundDesign)))
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
        return type == typeof(SoundDesign) || type.IsSubclassOf(typeof(SoundDesign));
    }

    /// <summary>
    /// Checks if type is derived from List<SoundDesign>
    /// </summary>
    private bool IsListOfSounds(Type? type)
    {
        var t = type;
        while (t != null)
        {
            if (t.IsGenericType &&
                t.GetGenericTypeDefinition() == typeof(List<>) &&
                typeof(SoundDesign).IsAssignableFrom(t.GetGenericArguments()[0]))
            {
                return true;
            }
            t = t.BaseType;
        }
        return false;
    }
}