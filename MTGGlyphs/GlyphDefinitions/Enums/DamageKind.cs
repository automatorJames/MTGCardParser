namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether only combat or noncombat damage is meant: "combat damage", "noncombat damage", in "prevent all combat damage that would be dealt by target creature this turn", "would deal noncombat damage to an opponent".</summary>
public enum DamageKind
{
    Noncombat,
    Combat
}
