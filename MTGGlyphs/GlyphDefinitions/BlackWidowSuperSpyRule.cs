namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Black Widow, Super Spy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Black Widow, Super Spy</exampledoc>
/// <examplecapture>if you don't, you may cast the exiled nonland card until end of turn and mana of any type can be spent to cast that spell</examplecapture>
[TokenizationOrder(-1)]
public class BlackWidowSuperSpyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't, you may cast the exiled nonland card until end of turn and mana of any type can be spent to cast that spell"];
}
