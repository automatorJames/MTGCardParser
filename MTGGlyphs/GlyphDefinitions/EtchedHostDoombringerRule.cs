namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Etched Host Doombringer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Etched Host Doombringer</exampledoc>
[TokenizationOrder(-1)]
public class EtchedHostDoombringerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent protects it, remove three defense counters from it"];
}
