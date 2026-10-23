namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Geralf, the Fleshwright, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Geralf, the Fleshwright</exampledoc>
/// <examplecapture>create a 2/2 blue and black zombie rogue creature token</examplecapture>
[Dependent]
public class GeralfTheFleshwrightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 2/2 blue and black zombie rogue creature token"];
}
