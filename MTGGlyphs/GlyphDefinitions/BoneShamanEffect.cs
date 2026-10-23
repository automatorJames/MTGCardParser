namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bone Shaman, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bone Shaman</exampledoc>
[Dependent]
public class BoneShamanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, this creature gains \"creatures dealt damage by this creature this turn can't be regenerated this turn"];
}
