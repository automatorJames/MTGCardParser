namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Old Hob, Alleycat Blues, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Old Hob, Alleycat Blues</exampledoc>
[Dependent]
public class OldHobAlleycatBluesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target attacking creature token gains indestructible until end of turn"];
}
