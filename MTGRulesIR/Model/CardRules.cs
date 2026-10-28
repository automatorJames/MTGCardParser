namespace MTGRulesIR.Model;

/// <summary>One card's rules, lowered: one <see cref="LineRules"/> per line of its text.</summary>
public sealed record CardRules(string Name, string Types, IReadOnlyList<LineRules> Lines)
{
    /// <summary>Whether every line lowered without an opaque node.</summary>
    public bool FullyLowered => Lines.All(x => x.FullyLowered);
}

/// <summary>
/// One line of a card's text, lowered: the abilities it states (one per top-level glyph, in text order), and the
/// glyph types of whatever in them stayed opaque - empty when the line is fully lowered.
/// </summary>
public sealed record LineRules(int Line, string Text, IReadOnlyList<Ability> Abilities, IReadOnlyList<string> Opaque)
{
    public bool FullyLowered => Opaque.Count == 0;
}
