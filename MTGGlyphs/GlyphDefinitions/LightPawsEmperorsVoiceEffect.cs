namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Light-Paws, Emperor's Voice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Light-Paws, Emperor's Voice</exampledoc>
/// <examplecapture>if you cast it, you may search your library for an aura card with mana value less than or equal to that aura and with a different name than each aura you control, put that card onto the battlefield attached to light-paws, then shuffle</examplecapture>
[Dependent]
public class LightPawsEmperorsVoiceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast it, you may search your library for an aura card with mana value less than or equal to that aura and with a different name than each aura you control, put that card onto the battlefield attached to light-paws, then shuffle"];
}
