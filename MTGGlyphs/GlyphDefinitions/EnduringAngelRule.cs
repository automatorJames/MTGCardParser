namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Enduring Angel // Angelic Enforcer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Enduring Angel // Angelic Enforcer</exampledoc>
[TokenizationOrder(-1)]
public class EnduringAngelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if your life total would be reduced to 0 or less, instead transform this creature and your life total becomes 3"];
}
