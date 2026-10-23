namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fire Lord Zuko, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fire Lord Zuko</exampledoc>
/// <examplecapture>put a +1/+1 counter on each creature you control</examplecapture>
[Dependent]
public class FireLordZukoEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on each creature you control"];
}
