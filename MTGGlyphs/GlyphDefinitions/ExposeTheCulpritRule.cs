namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Expose the Culprit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Expose the Culprit</exampledoc>
[TokenizationOrder(-1)]
public class ExposeTheCulpritRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• exile any number of face-up creatures you control with disguise in a face-down pile, shuffle that pile, then cloak them"];
}
