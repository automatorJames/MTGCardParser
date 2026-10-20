namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The leyline rule: a card that may start the game on the battlefield - "if this card is in your opening hand, you may begin the game with it on the battlefield".</summary>
/// <exampledoc>Leyline of Anticipation</exampledoc>
/// <examplecapture>if this card is in your opening hand, you may begin the game with it on the battlefield</examplecapture>
public class BeginGameOnBattlefield : Glyph
{
    public override Nib[] Nibs => ["if this card is in your opening hand, you may begin the game with it on the battlefield"];
}
