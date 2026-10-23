namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kitsune Mystic // Autumn-Tail, Kitsune Sage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kitsune Mystic // Autumn-Tail, Kitsune Sage</exampledoc>
[Dependent]
public class KitsuneMysticEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature is enchanted by two or more auras, flip it"];
}
