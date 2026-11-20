using System.Text.Json.Serialization;

namespace MTGRulesIR.Model;

/// <summary>
/// One card's rules, lowered: one <see cref="LineRules"/> per line of its text, and for a two-faced card the back face's
/// rules as a card of their own (named by the back face, so "{this}" and line overrides are the back face's), under
/// <see cref="Back"/>.
/// </summary>
public sealed record CardRules(string Name, string Types, IReadOnlyList<LineRules> Lines, CardInfo Info = null, CardRules Back = null)
{
    /// <summary>Whether every line of every face lowered without an opaque node.</summary>
    public bool FullyLowered => AllLines.All(x => x.FullyLowered);

    /// <summary>Whether every line of every face is engine-closed (see <see cref="LineRules.EngineClosed"/>).</summary>
    public bool EngineClosed => AllLines.All(x => x.EngineClosed);

    /// <summary>This face and the back face, where there is one.</summary>
    [JsonIgnore]
    public IEnumerable<CardRules> Faces => Back is null ? [this] : [this, Back];

    /// <summary>The lines of every face.</summary>
    [JsonIgnore]
    public IEnumerable<LineRules> AllLines => Faces.SelectMany(x => x.Lines);
}

/// <summary>
/// One line of a card's text, lowered: the abilities it states (one per top-level glyph, in text order), the glyph
/// types of whatever in them stayed opaque - empty when the line is fully lowered - and the open slots (see
/// <see cref="Lowering.OpenSlots"/>): untyped slots holding a node, and placeholders, each named
/// <c>Record.Property</c> - empty when the line is engine-closed.
/// </summary>
public sealed record LineRules(int Line, string Text, IReadOnlyList<Ability> Abilities, IReadOnlyList<string> Opaque, IReadOnlyList<string> Open = null)
{
    public bool FullyLowered => Opaque.Count == 0;

    /// <summary>
    /// Whether the engine could act on this line without reading text or guessing: nothing opaque, nothing in an
    /// untyped slot, no placeholder. Fully lowered is the structural measure; this is the stricter, semantic one.
    /// </summary>
    public bool EngineClosed => FullyLowered && (Open?.Count ?? 0) == 0;
}

/// <summary>
/// What a card is besides its text, as the database gives it: mana cost ("{1}{G}"), supertypes, subtypes, power,
/// toughness and loyalty as printed (so "*" and "X" survive), and the keywords the database lists for it.
/// </summary>
public sealed record CardInfo(string ManaCost = null, string Supertypes = null, string Subtypes = null, string Power = null, string Toughness = null, string Loyalty = null, IReadOnlyList<string> Keywords = null, string Colors = null);
