namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Illusionary Mask, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illusionary Mask</exampledoc>
/// <examplecapture>if the creature that spell becomes as it resolves has not been turned face up and would assign or deal damage, be dealt damage, or become tapped, instead it's turned face up and assigns or deals damage, is dealt damage, or becomes tapped</examplecapture>
[TokenizationOrder(-1)]
public class IllusionaryMaskRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the creature that spell becomes as it resolves has not been turned face up and would assign or deal damage, be dealt damage, or become tapped, instead it's turned face up and assigns or deals damage, is dealt damage, or becomes tapped"];
}
