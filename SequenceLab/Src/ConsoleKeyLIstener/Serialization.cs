using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Beater;

public static class Serialization
{
    public static string ToJson(this Sequence seq)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new TypedBaseClassesContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
        return JsonConvert.SerializeObject(seq, Formatting.Indented, settings);
    }

    public static T? FromJson<T>(this string json)
    {
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        return JsonConvert.DeserializeObject<T>(json, settings);
    }
}

/// <summary>
/// Add information about concrete type to JSON only if current property is a base class (or an interface).
/// All other classes, including system types are added without type info.
/// </summary>
public class TypedBaseClassesContractResolver : DefaultContractResolver
{
    protected override JsonContract CreateContract(Type objectType)
    {
        JsonContract contract = base.CreateContract(objectType);

        if (contract is JsonObjectContract obj)
        {
            foreach (var p in obj.Properties)
            {
                if (!IsSystem(p.PropertyType) && IsBase(p.PropertyType))
                {
                    // add "$type" field only for custom types with derived classes:
                    p.TypeNameHandling = TypeNameHandling.All;
                }
            }
        }

        if (contract is JsonContainerContract list)
        {
            if (!IsSystem(objectType) && IsBase(objectType))
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
        return Assembly.GetAssembly(type)?.GetTypes().Any(t => t.BaseType == type) ?? false;
    }
}