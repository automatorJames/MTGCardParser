namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shape of the Wiitigo, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shape of the Wiitigo</exampledoc>
[Dependent]
public class ShapeOfTheWiitigoEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on enchanted creature if it attacked or blocked since your last upkeep"];
}
