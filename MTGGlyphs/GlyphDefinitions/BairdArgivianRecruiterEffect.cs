namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Baird, Argivian Recruiter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Baird, Argivian Recruiter</exampledoc>
/// <examplecapture>if you control a creature with power greater than its base power, create a 1/1 white soldier creature token</examplecapture>
[Dependent]
public class BairdArgivianRecruiterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control a creature with power greater than its base power, create a 1/1 white soldier creature token"];
}
