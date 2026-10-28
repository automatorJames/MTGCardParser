namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Return Triumphant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Return Triumphant</exampledoc>
[TokenizationOrder(-1)]
public class ReturnTriumphantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put another role on the creature later, put this one into the graveyard"];
}
