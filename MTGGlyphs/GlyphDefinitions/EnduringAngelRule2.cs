namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Enduring Angel // Angelic Enforcer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Enduring Angel // Angelic Enforcer</exampledoc>
[TokenizationOrder(-1)]
public class EnduringAngelRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if this creature didn't transform this way, you lose the game"];
}
