namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which damage can't be prevented: "the" damage, "that" damage, or "combat" damage; one optional enum, since an optional enum followed by an optional bool never matched.</summary>
public enum DamageScope
{
    The,
    That,
    Combat
}
