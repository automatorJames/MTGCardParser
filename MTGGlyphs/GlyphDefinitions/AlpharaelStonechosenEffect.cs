namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Alpharael, Stonechosen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alpharael, Stonechosen</exampledoc>
/// <examplecapture>if a nonland permanent left the battlefield this turn or a spell was warped this turn, defending player loses half their life, rounded up</examplecapture>
[Dependent]
public class AlpharaelStonechosenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a nonland permanent left the battlefield this turn or a spell was warped this turn, defending player loses half their life, rounded up"];
}
