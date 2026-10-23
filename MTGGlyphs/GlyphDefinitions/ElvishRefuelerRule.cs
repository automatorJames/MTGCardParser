namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elvish Refueler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elvish Refueler</exampledoc>
/// <examplecapture>during your turn, as long as you haven't activated an exhaust ability this turn, you may activate exhaust abilities as though they haven't been activated</examplecapture>
[TokenizationOrder(-1)]
public class ElvishRefuelerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn, as long as you haven't activated an exhaust ability this turn, you may activate exhaust abilities as though they haven't been activated"];
}
