namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Edgar's Awakening, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Edgar's Awakening</exampledoc>
[Dependent]
public class EdgarsAwakeningEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may pay {b}"];
}
