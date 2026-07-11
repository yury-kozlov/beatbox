using System.Diagnostics;

namespace Beater;

[DebuggerDisplay("{ToString(),nq}")]
public class Tags : List<string>
{
    public override string ToString() => "[" + string.Join(", ", this) + "]";
}
