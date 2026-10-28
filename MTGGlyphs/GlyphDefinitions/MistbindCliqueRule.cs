namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mistbind Clique, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mistbind Clique</exampledoc>
[TokenizationOrder(-1)]
public class MistbindCliqueRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when a faerie is championed with this creature, tap all lands target player controls"];
}
