namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ooze Flux, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ooze Flux</exampledoc>
[TokenizationOrder(-1)]
public class OozeFluxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{1}{g}, remove one or more +1/+1 counters from among creatures you control: create an x/x green ooze creature token, where x is the number of +1/+1 counters removed this way"];
}
