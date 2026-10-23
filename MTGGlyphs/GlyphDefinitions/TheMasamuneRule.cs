namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Masamune, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Masamune</exampledoc>
/// <examplecapture>equipped creature has "if a creature dying causes a triggered ability of this creature or an emblem you own to trigger, that ability triggers an additional time</examplecapture>
[TokenizationOrder(-1)]
public class TheMasamuneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature has \"if a creature dying causes a triggered ability of this creature or an emblem you own to trigger, that ability triggers an additional time"];
}
