namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cowardice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cowardice</exampledoc>
[Dependent]
public class CowardiceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it won't be affected by the spell or ability"];
}
