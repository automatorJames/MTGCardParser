namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Guardian Angel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guardian Angel</exampledoc>
[TokenizationOrder(-1)]
public class GuardianAngelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, you may pay {1} any time you could cast an instant"];
}
