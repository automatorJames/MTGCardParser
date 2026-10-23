namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Blow Your House Down, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blow Your House Down</exampledoc>
[TokenizationOrder(-1)]
public class BlowYourHouseDownRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy any of them that are walls"];
}
