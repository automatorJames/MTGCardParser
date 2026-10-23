namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Veteran's Voice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Veteran's Voice</exampledoc>
[Dependent]
public class VeteransVoiceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature other than the creature tapped this way gets +2/+1 until end of turn"];
}
