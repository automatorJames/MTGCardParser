namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Food Fight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Food Fight</exampledoc>
/// <examplecapture>artifacts you control have "{2}, sacrifice this artifact: it deals damage to any target equal to 1 plus the number of permanents named {this} you control</examplecapture>
[TokenizationOrder(-1)]
public class FoodFightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["artifacts you control have \"{2}, sacrifice this artifact: it deals damage to any target equal to 1 plus the number of permanents named", Nib.This, "you control"];
}
