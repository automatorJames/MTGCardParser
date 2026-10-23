namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spry and Mighty, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spry and Mighty</exampledoc>
/// <examplecapture>you draw x cards and the chosen creatures get +x/+x and gain trample until end of turn, where x is the difference between the chosen creatures' powers</examplecapture>
[TokenizationOrder(-1)]
public class SpryAndMightyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you draw x cards and the chosen creatures get +x/+x and gain trample until end of turn, where x is the difference between the chosen creatures' powers"];
}
