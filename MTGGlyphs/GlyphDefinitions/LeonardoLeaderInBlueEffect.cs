namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Leonardo, Leader in Blue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Leonardo, Leader in Blue</exampledoc>
[Dependent]
public class LeonardoLeaderInBlueEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if his sneak cost was paid, creatures you control get +2/+0 until end of turn"];
}
