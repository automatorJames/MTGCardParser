namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Foul Emissary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Foul Emissary</exampledoc>
[TokenizationOrder(-1)]
public class FoulEmissaryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you sacrifice this creature while casting a spell with emerge, create a 3/2 colorless eldrazi horror creature token"];
}
