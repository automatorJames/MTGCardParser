namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grifter's Blade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grifter's Blade</exampledoc>
[TokenizationOrder(-1)]
public class GriftersBladeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as this equipment enters, choose a creature you control it could be attached to"];
}
