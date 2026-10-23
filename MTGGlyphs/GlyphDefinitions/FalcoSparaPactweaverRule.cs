namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Falco Spara, Pactweaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Falco Spara, Pactweaver</exampledoc>
/// <examplecapture>you may cast spells from the top of your library by removing a counter from a creature you control in addition to paying their other costs</examplecapture>
[TokenizationOrder(-1)]
public class FalcoSparaPactweaverRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast spells from the top of your library by removing a counter from a creature you control in addition to paying their other costs"];
}
