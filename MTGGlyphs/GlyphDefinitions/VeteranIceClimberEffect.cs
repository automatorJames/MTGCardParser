namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Veteran Ice Climber, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Veteran Ice Climber</exampledoc>
[Dependent]
public class VeteranIceClimberEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they put that many cards from the top of their library into their graveyard"];
}
