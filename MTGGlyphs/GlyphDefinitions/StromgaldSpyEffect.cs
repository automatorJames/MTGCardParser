namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Stromgald Spy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stromgald Spy</exampledoc>
/// <examplecapture>you may have defending player play with their hand revealed for as long as this creature remains on the battlefield</examplecapture>
[Dependent]
public class StromgaldSpyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have defending player play with their hand revealed for as long as this creature remains on the battlefield"];
}
