namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reptilian Recruiter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reptilian Recruiter</exampledoc>
/// <examplecapture>if that creature's power is 2 or less or if you control another lizard, gain control of that creature until end of turn, untap it, and it gains haste until end of turn</examplecapture>
[TokenizationOrder(-1)]
public class ReptilianRecruiterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that creature's power is 2 or less or if you control another lizard, gain control of that creature until end of turn, untap it, and it gains haste until end of turn"];
}
