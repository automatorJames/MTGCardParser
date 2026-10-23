namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ashiok, Wicked Manipulator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashiok, Wicked Manipulator</exampledoc>
/// <examplecapture>create two 1/1 black nightmare creature tokens with "at the beginning of combat on your turn, if a card was put into exile this turn, put a +1/+1 counter on this token</examplecapture>
[Dependent]
public class AshiokWickedManipulatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create two 1/1 black nightmare creature tokens with \"at the beginning of combat on your turn, if a card was put into exile this turn, put a +1/+1 counter on this token"];
}
