namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dawnhand Dissident, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dawnhand Dissident</exampledoc>
/// <examplecapture>during your turn, you may cast creature spells from among cards you own exiled with this creature by removing three counters from among creatures you control in addition to paying their other costs</examplecapture>
[TokenizationOrder(-1)]
public class DawnhandDissidentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn, you may cast creature spells from among cards you own exiled with this creature by removing three counters from among creatures you control in addition to paying their other costs"];
}
