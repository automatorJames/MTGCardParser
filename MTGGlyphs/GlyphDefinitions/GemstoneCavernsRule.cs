namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gemstone Caverns, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gemstone Caverns</exampledoc>
/// <examplecapture>if this card is in your opening hand and you're not the starting player, you may begin the game with {this} on the battlefield with a luck counter on it</examplecapture>
[TokenizationOrder(-1)]
public class GemstoneCavernsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this card is in your opening hand and you're not the starting player, you may begin the game with", Nib.This, "on the battlefield with a luck counter on it"];
}
