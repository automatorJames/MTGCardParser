namespace MTGRulesIR.Model;

/// <summary>One card's rules, lowered: one <see cref="LineRules"/> per line of its text.</summary>
public sealed record CardRules(string Name, string Types, IReadOnlyList<LineRules> Lines, CardInfo Info = null)
{
    /// <summary>Whether every line lowered without an opaque node.</summary>
    public bool FullyLowered => Lines.All(x => x.FullyLowered);

    /// <summary>Whether every line is engine-closed (see <see cref="LineRules.EngineClosed"/>).</summary>
    public bool EngineClosed => Lines.All(x => x.EngineClosed);
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
public sealed record CardInfo(string ManaCost = null, string Supertypes = null, string Subtypes = null, string Power = null, string Toughness = null, string Loyalty = null, IReadOnlyList<string> Keywords = null);
