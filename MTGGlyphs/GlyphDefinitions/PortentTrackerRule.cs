namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Portent Tracker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Portent Tracker</exampledoc>
[TokenizationOrder(-1)]
public class PortentTrackerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent protects it, remove a defense counter from it"];
}
