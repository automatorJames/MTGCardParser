namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ashiok, Wicked Manipulator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashiok, Wicked Manipulator</exampledoc>
/// <examplecapture>if you would pay life while your library has at least that many cards in it, exile that many cards from the top of your library instead</examplecapture>
[TokenizationOrder(-1)]
public class AshiokWickedManipulatorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would pay life while your library has at least that many cards in it, exile that many cards from the top of your library instead"];
}
