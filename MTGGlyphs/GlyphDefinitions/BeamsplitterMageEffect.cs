namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Beamsplitter Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Beamsplitter Mage</exampledoc>
/// <examplecapture>if you control one or more other creatures that spell could target, choose one of those creatures</examplecapture>
[Dependent]
public class BeamsplitterMageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control one or more other creatures that spell could target, choose one of those creatures"];
}
