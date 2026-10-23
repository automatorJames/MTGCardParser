namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chandra, Chill of Compliance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra, Chill of Compliance</exampledoc>
[TokenizationOrder(-1)]
public class ChandraChillOfComplianceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put a noncreature, nonland card into your graveyard this way, put that card into your hand"];
}
