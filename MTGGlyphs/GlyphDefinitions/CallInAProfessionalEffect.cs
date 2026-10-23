namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Call In a Professional, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Call In a Professional</exampledoc>
[Dependent]
public class CallInAProfessionalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shield counters don't prevent this damage as they're removed"];
}
