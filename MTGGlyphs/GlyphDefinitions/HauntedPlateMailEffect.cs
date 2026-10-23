namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Haunted Plate Mail, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haunted Plate Mail</exampledoc>
[Dependent]
public class HauntedPlateMailEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, this permanent becomes a 4/4 spirit artifact creature that's no longer an equipment"];
}
