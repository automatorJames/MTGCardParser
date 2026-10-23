namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Winterthorn Blessing, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winterthorn Blessing</exampledoc>
[TokenizationOrder(-1)]
public class WinterthornBlessingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap up to one target creature you don't control, and that creature doesn't untap during its controller's next untap step"];
}
