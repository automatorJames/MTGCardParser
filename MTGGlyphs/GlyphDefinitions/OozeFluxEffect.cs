namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ooze Flux, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ooze Flux</exampledoc>
/// <examplecapture>create an x/x green ooze creature token, where x is the number of +1/+1 counters removed this way</examplecapture>
[Dependent]
public class OozeFluxEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create an x/x green ooze creature token, where x is the number of +1/+1 counters removed this way"];
}
