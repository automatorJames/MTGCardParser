namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nacatl War-Pride, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nacatl War-Pride</exampledoc>
/// <examplecapture>create x tokens that are copies of it and that are tapped and attacking, where x is the number of creatures defending player controls</examplecapture>
[Dependent]
public class NacatlWarPrideEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create x tokens that are copies of it and that are tapped and attacking, where x is the number of creatures defending player controls"];
}
