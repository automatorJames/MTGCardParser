namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Indestructibility, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Indestructibility</exampledoc>
[Dependent]
public class IndestructibilityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["effects that say \"destroy\" don't destroy that permanent"];
}
