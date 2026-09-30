using System.Globalization;

namespace Glyphotype.RegexGeneration.Graph;

/// <summary>
/// A CLR primitive the engine can capture as a terminal: an open-vocabulary value parsed from the matched
/// text, as opposed to an enum's closed vocabulary (the text is one of a fixed set of words) or a bool's
/// presence flag (the text is there or it isn't).
/// <para>
/// The one place that says which primitives are supported and how - node selection
/// (<see cref="GlyphNode.GetNodeForNavigaton"/>), property discovery (<see cref="PropertyNib"/>) and
/// validation all consult it rather than naming types themselves. Only read while regex graphs are built:
/// each <see cref="PrimitiveNode"/> takes its entry once, so matching never touches this table. Supporting a
/// new primitive is one entry here - bearing in mind the Tokenizer treats every period as a clause break, so
/// a type written with one (a decimal) needs that addressed first.
/// </para>
/// </summary>
/// <param name="DisplayName">The C# keyword for the type, for display.</param>
/// <param name="DefaultPattern">The regex matched when the property declares no <see cref="RegexPatternAttribute"/>.</param>
/// <param name="Parse">The CLR value of the matched text, or null if it doesn't parse (e.g. overflows), which fails hydration.</param>
public sealed record PrimitiveTerminal(string DisplayName, string DefaultPattern, Func<string, object> Parse)
{
    static readonly Dictionary<Type, PrimitiveTerminal> _byType = new()
    {
        [typeof(int)] = new("int", @"\d+", text => int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value) ? value : null),

        // Todo: candidates, to add when a corpus needs them. Each is one entry, but settle its open question first.
        // [typeof(long)] = new("long", @"\d+", ...),              // Q: worth it over int? Corpora with ids, populations, currency in cents
        // [typeof(decimal)] = new("decimal", @"\d+\.\d+", ...),   // Blocked: the Tokenizer treats every "." as a clause break, splitting "3.5" first
        // [typeof(double)] = new("double", @"\d+\.\d+", ...),     // Same blocker as decimal; also Q: when would anyone want double over decimal here?
        // [typeof(DateOnly)] = new("date", ..., ...),             // Q: which formats? Culture-dependent (3/4 vs 4/3), and "." again in "4.3.2026"
        // [typeof(TimeOnly)] = new("time", @"\d{1,2}:\d{2}", ...),// Q: 12h ("9:30 pm") vs 24h; am/pm may belong to the pattern or to a sibling enum
        // [typeof(TimeSpan)] = new("duration", ..., ...),         // Q: "2 hours" is really int + enum; only worth it for "02:30"-style text
        // Deliberately absent: string. With no closed vocabulary or format to bound it, it's a wildcard - that's DynamicGlyph's job.
    };

    /// <summary>The terminal for <paramref name="type"/> (nullable-unwrapped), if it's a supported primitive.</summary>
    public static bool TryGet(Type type, out PrimitiveTerminal terminal) =>
        _byType.TryGetValue(Nullable.GetUnderlyingType(type) ?? type, out terminal);

    public static bool IsSupported(Type type) => TryGet(type, out _);

    /// <summary>The supported primitive whose <see cref="DisplayName"/> is <paramref name="displayName"/> (e.g. "int").</summary>
    public static bool TryGetType(string displayName, out Type type)
    {
        type = _byType.FirstOrDefault(x => x.Value.DisplayName == displayName).Key;
        return type is not null;
    }

    public static IEnumerable<string> SupportedDisplayNames => _byType.Values.Select(x => x.DisplayName);
}
