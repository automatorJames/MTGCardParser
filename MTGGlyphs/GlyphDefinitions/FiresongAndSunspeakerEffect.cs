namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Firesong and Sunspeaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Firesong and Sunspeaker</exampledoc>
[Dependent]
public class FiresongAndSunspeakerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 3 damage to target creature or player"];
}
