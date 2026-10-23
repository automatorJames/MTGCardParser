namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ambitious Augmenter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ambitious Augmenter</exampledoc>
/// <examplecapture>if it had one or more counters on it, create a 0/0 green and blue fractal creature token, then put this creature's counters on that token</examplecapture>
[Dependent]
public class AmbitiousAugmenterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it had one or more counters on it, create a 0/0 green and blue fractal creature token, then put this creature's counters on that token"];
}
