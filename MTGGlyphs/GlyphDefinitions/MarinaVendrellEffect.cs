namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Marina Vendrell, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Marina Vendrell</exampledoc>
[Dependent]
public class MarinaVendrellEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lock or unlock a door of target room you control"];
}
